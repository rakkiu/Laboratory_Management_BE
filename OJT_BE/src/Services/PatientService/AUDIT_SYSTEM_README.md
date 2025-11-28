# MediatR Behavior-based Audit System

## ?? T?ng quan

H? th?ng audit logging t? ??ng s? d?ng **MediatR Pipeline Behavior** ?? ghi l?i t?t c? các thay ??i trên `PatientMedicalRecord` vào b?ng `PatientRecordAuditLog`.

## ??? Ki?n trúc

```
Client ? Controller ? MediatR ? AuditBehavior ? Handler ? DbContext ? Database
                                      ?
                                 Transaction
                                      ?
                          Business Changes + Audit Logs
```

### Lu?ng x? lý (Sequence):

1. **Client** g?i API endpoint
2. **Controller** g?i `mediator.Send(command)`
3. **AuditBehavior** (Pre-processing):
   - G?i `ChangeTracker.DetectChanges()`
   - Capture OLD values (cho UPDATE)
4. **Handler** th?c thi business logic
   - ? **KHÔNG** g?i `SaveChangesAsync()`
5. **AuditBehavior** (Post-processing):
   - Capture NEW values
   - B?t ??u Transaction
   - `SaveChangesAsync()` - l?u business changes
   - T?o audit log entries
   - `SaveChangesAsync()` - l?u audit logs
   - Commit Transaction ?
6. Response tr? v? client

## ?? Cách s? d?ng

### 1. ?ánh d?u Command c?n audit

Implement interface `IAuditableCommand`:

```csharp
public class CreatePatientMedicalRecordCommand : IRequest<CreatePatientMedicalRecord>, IAuditableCommand
{
    // ... properties ...
    
    // IAuditableCommand implementation
    public Guid PerformedBy => CreatedBy;
    public string GetAuditAction() => "CREATE";
    public Guid? GetEntityId() => null;
}
```

### 2. Handler KHÔNG g?i SaveChangesAsync()

```csharp
public class CreatePatientMedicalRecordHandler : IRequestHandler<...>
{
    public async Task<...> Handle(...)
    {
        // Add/Update entities
        await _repository.AddMedicalRecordAsync(record);
        
        // ? KHÔNG g?i SaveChangesAsync() ? ?ây
        // AuditBehavior s? x? lý
        
        return response;
    }
}
```

### 3. AuditBehavior t? ??ng x? lý

- ? T? ??ng capture changes
- ? T? ??ng ghi audit logs
- ? Atomic transaction (rollback n?u có l?i)

## ?? Audit Log Format

### CREATE Action:
```json
{
  "AuditId": "guid",
  "RecordId": "record-guid",
  "Action": "CREATE",
  "PerformedBy": "user-guid",
  "Timestamp": "2025-11-03T10:00:00Z",
  "ChangedFields": null,
  "OldValues": null,
  "NewValues": "{\"RecordId\":\"...\",\"PatientId\":\"...\",\"ClinicalNotes\":\"...\"}"
}
```

### UPDATE Action:
```json
{
  "AuditId": "guid",
  "RecordId": "record-guid",
  "Action": "UPDATE",
  "PerformedBy": "user-guid",
  "Timestamp": "2025-11-03T11:00:00Z",
  "ChangedFields": "[\"Diagnosis\",\"ClinicalNotes\"]",
  "OldValues": "{\"Diagnosis\":\"Old\",\"ClinicalNotes\":\"Old notes\"}",
  "NewValues": "{\"Diagnosis\":\"New\",\"ClinicalNotes\":\"New notes\"}"
}
```

### DELETE Action:
```json
{
  "AuditId": "guid",
  "RecordId": "record-guid",
  "Action": "DELETE",
  "PerformedBy": "user-guid",
  "Timestamp": "2025-11-03T12:00:00Z",
  "ChangedFields": null,
  "OldValues": "{\"RecordId\":\"...\",\"IsDeleted\":false}",
  "NewValues": null
}
```

## ?? API Endpoints

### Create Medical Record
```http
POST /api/medicalrecord
Content-Type: application/json

{
  "patient": {
    "fullName": "Nguy?n V?n A",
    "dateOfBirth": "1990-01-01",
    "gender": "Male",
    "phoneNumber": "0123456789",
    "email": "test@example.com",
    "address": "123 Street"
  },
  "clinicalNotes": "Patient notes",
  "diagnosis": "Diagnosis",
  "doctorId": "doctor-guid",
  "createdBy": "user-guid"
}
```

### Update Medical Record
```http
PUT /api/medicalrecord/{recordId}
Content-Type: application/json

{
  "clinicalNotes": "Updated notes",
  "diagnosis": "Updated diagnosis",
  "doctorId": "doctor-guid",
  "updatedBy": "user-guid"
}
```

### Delete Medical Record (Soft Delete)
```http
DELETE /api/medicalrecord/{recordId}?deletedBy=user-guid
```

### Get All Medical Records
```http
GET /api/medicalrecord
```

## ? ?u ?i?m

1. **?? Centralized Logic**: T?t c? audit logic ? m?t ch?
2. **?? Atomic Transactions**: Business + Audit ???c l?u cùng transaction
3. **?? Automatic Tracking**: T? ??ng track changes
4. **?? Reusable**: Thêm `IAuditableCommand` là t? ??ng có audit
5. **?? Testable**: Handlers và AuditBehavior test riêng
6. **?? Complete History**: Track OLD và NEW values ??y ??
7. **?? No Code Duplication**: Handlers ch? ch?a business logic

## ?? B?o m?t & Encryption

- ? Patient data ???c **encrypt khi l?u** vào DB
- ? Patient data ???c **decrypt khi ??c** t? DB
- ? Audit logs l?u **encrypted data** (không decrypt trong audit)
- ? Response API tr? **plain text** data (?ã decrypt)

## ??? Clean Architecture

```
Presentation Layer (API Controllers)
        ?
Application Layer (Handlers + Behaviors)
        ?
Domain Layer (Entities + Interfaces)
        ?
Infrastructure Layer (Repositories + DbContext)
```

## ?? Dependencies

- **MediatR**: Pipeline behavior support
- **Entity Framework Core**: Database access + Change Tracking
- **FluentValidation**: Request validation
- **Npgsql**: PostgreSQL provider

## ?? Deployment

1. Ensure encryption key is configured in `appsettings.json`:
```json
{
  "EnvironmentVariables": {
    "DATA_ENCRYPTION_KEY": "your-base64-key-here"
  }
}
```

2. Run migrations:
```bash
dotnet ef database update --project src/PatientService.Infrastructure --startup-project src/PatientService.Presentation
```

3. Run application:
```bash
dotnet run --project src/PatientService.Presentation
```

## ?? Notes

- Handlers **KHÔNG ???c** g?i `SaveChangesAsync()` tr?c ti?p
- `AuditBehavior` ch? audit commands implement `IAuditableCommand`
- Query operations không ???c audit (read-only)
- Audit logs bao g?m c? encrypted data ?? tracking ??y ??

## ?? Best Practices

1. ? **Luôn implement `IAuditableCommand`** cho commands c?n audit
2. ? **KHÔNG g?i `SaveChangesAsync()`** trong handlers
3. ? **Set `PerformedBy`** t? authenticated user
4. ? **Use transactions** cho complex operations
5. ? **Log errors** trong audit behavior
6. ? **Monitor audit logs** regularly

---

Made with ?? using MediatR Pipeline Behaviors
