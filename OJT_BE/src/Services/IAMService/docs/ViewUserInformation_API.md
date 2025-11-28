# View User Information API Documentation

## Overview
API endpoint to retrieve detailed information about a user. Only accessible by Admin and Lab Manager roles.

---

## Endpoint
```
POST /api/auth/viewUserInformation
```

## Authorization
- **Required Roles**: Admin, Lab Manager (LAB_MANAGER)
- **Authentication**: JWT Token in request body

---

## Request

### Headers
```
Content-Type: application/json
```

### Body Parameters

| Field | Type | Required | Description | Example |
|-------|------|----------|-------------|---------|
| `token` | string | Yes | JWT authentication token | `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...` |
| `userId` | UUID | Yes | ID of the user to view | `550e8400-e29b-41d4-a716-446655440000` |

### Request Example
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c",
  "userId": "550e8400-e29b-41d4-a716-446655440000"
}
```

---

## Response

### Success Response (200 OK)

```json
{
  "data": {
    "userId": "550e8400-e29b-41d4-a716-446655440000",
    "fullName": "Nguyen Van A",
    "email": "nguyenvana@example.com",
    "phoneNumber": "+84901234567",
    "identifyNumber": "079203001234",
    "gender": "Male",
    "age": 29,
    "address": "123 Nguyen Trai, Ha Noi",
    "dateOfBirth": "07/12/1996",
    "role": {
      "roleName": "Manager",
      "roleCode": "LAB_MANAGER",
      "roleDescription": "Has permission to view and manage user accounts"
    }
  },
  "statusCode": 200,
  "message": "View user information successfully",
  "responsedAt": "2024-01-20T10:30:00Z"
}
```

### Response Fields

| Field | Type | Description |
|-------|------|-------------|
| `data.userId` | UUID | User's unique identifier |
| `data.fullName` | string | User's full name |
| `data.email` | string | User's email address |
| `data.phoneNumber` | string | User's phone number |
| `data.identifyNumber` | string | User's identification number (CCCD/CMND) |
| `data.gender` | string | User's gender (Male/Female) |
| `data.age` | integer | User's age |
| `data.address` | string | User's address |
| `data.dateOfBirth` | string | User's date of birth (format: dd/MM/yyyy) |
| `data.role.roleName` | string | Name of user's role |
| `data.role.roleCode` | string | Code of user's role |
| `data.role.roleDescription` | string | Description of user's role |

---

## Error Responses

### 400 Bad Request - Token Missing
```json
{
  "data": null,
  "statusCode": 400,
  "message": "Validation failed",
  "responsedAt": "2024-01-20T10:30:00Z"
}
```

**Cause**: Token field is empty or not provided

---

### 401 Unauthorized - Invalid Token
```json
{
  "data": null,
  "statusCode": 401,
  "message": "Invalid or expired token.",
  "responsedAt": "2024-01-20T10:30:00Z"
}
```

**Causes**:
- Token is invalid
- Token has expired
- Token signature is invalid

---

### 401 Unauthorized - Insufficient Permissions
```json
{
  "data": null,
  "statusCode": 401,
  "message": "Unauthorized. Please log in again.",
  "responsedAt": "2024-01-20T10:30:00Z"
}
```

**Causes**:
- User role is not Admin or Lab Manager
- Token doesn't contain required claims

---

### 404 Not Found - User Not Found
```json
{
  "data": null,
  "statusCode": 404,
  "message": "User not found or has been deleted.",
  "responsedAt": "2024-01-20T10:30:00Z"
}
```

**Causes**:
- User with specified ID doesn't exist
- User account has been deactivated (IsActive = false)

---

### 500 Internal Server Error
```json
{
  "data": null,
  "statusCode": 500,
  "message": "An error occurred while retrieving user information.",
  "responsedAt": "2024-01-20T10:30:00Z"
}
```

**Cause**: Unexpected server error

---

## Implementation Details

### Architecture Layers

1. **Presentation Layer**
   - `AuthController.ViewUserInformation()` - API endpoint

2. **Application Layer**
   - `ViewUserInformationCommand` - Request model
   - `ViewUserInformationHandler` - Business logic
   - `ViewUserInformationValidator` - Input validation

3. **Domain Layer**
   - `User` entity with navigation to `Role`
   - `IUserRepository.GetByIdWithRoleAsync()` - Data access interface

4. **Infrastructure Layer**
   - `UserRepository.GetByIdWithRoleAsync()` - EF Core implementation
   - JWT token validation via `IJwtService`

### Validation Rules

| Field | Rules |
|-------|-------|
| `token` | Not empty |
| `userId` | Not empty, valid GUID format |

### Authorization Logic

1. Validate JWT token structure and signature
2. Extract `sub` (user ID) claim from token
3. Extract `RoleCode` claim from token
4. Check if `RoleCode` is either `ADMIN` or `LAB_MANAGER`
5. If any check fails → return 401 Unauthorized

### Database Query

```csharp
_db.Users
    .Include(u => u.Role)
    .Include(u => u.RefreshTokens)
    .FirstOrDefaultAsync(u => u.UserId == id);
```

---

## Testing

### Unit Tests Location
- `TestProject/ViewUserInformation/ViewUserInformationHandlerTests.cs`
- `TestProject/ViewUserInformation/ViewUserInformationValidatorTests.cs`

### Test HTTP Requests
See `test-viewUserInformation.http` file in the Presentation layer.

---

## Usage Example

### 1. Login to get token
```http
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "email": "admin@system.com",
  "password": "Admin@123"
}
```

### 2. Extract accessToken from response
```json
{
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "..."
  }
}
```

### 3. View user information
```http
POST http://localhost:5000/api/auth/viewUserInformation
Content-Type: application/json

{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": "550e8400-e29b-41d4-a716-446655440000"
}
```

---

## Notes

- Password field is **never** exposed in the response
- Only active users (IsActive = true) can be viewed
- Date format is dd/MM/yyyy (e.g., "07/12/1996")
- The API uses POST method as specified in the requirements (instead of RESTful GET)
- Token is sent in request body (not in Authorization header) as per specification

---

## Related APIs

- `POST /api/auth/login` - Authenticate and get token
- `POST /api/auth/logout` - Revoke refresh token

---

## Version History

- **v1.0** - Initial implementation (2024-01-20)
  - View user information for Admin and Lab Manager roles
  - Full validation and error handling
  - Unit tests coverage

