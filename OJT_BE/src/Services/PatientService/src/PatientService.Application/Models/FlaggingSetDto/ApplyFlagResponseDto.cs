namespace PatientService.Application.Models.FlaggingSetDto
{
    /// <summary>
    /// 
    /// </summary>
    public class ApplyFlagResponseDto
    {
        /// <summary>
        /// Gets or sets the result identifier.
        /// </summary>
        /// <value>
        /// The result identifier.
        /// </value>
        public Guid ResultId { get; set; }
        /// <summary>
        /// Gets or sets the name of the test.
        /// </summary>
        /// <value>
        /// The name of the test.
        /// </value>
        public string TestName { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the flag.
        /// </summary>
        /// <value>
        /// The flag.
        /// </value>
        public string? Flag { get; set; }
        /// <summary>
        /// Gets or sets the reference range.
        /// </summary>
        /// <value>
        /// The reference range.
        /// </value>
        public string? ReferenceRange { get; set; }
        /// <summary>
        /// Gets or sets the interpretation.
        /// </summary>
        /// <value>
        /// The interpretation.
        /// </value>
        public string? Interpretation { get; set; }
    }
}