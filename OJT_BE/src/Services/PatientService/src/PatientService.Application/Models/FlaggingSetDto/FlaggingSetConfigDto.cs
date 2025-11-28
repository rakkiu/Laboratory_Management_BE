using System;

namespace PatientService.Application.Models.FlaggingSetDto
{
    /// <summary>
    /// 
    /// </summary>
    public class FlaggingSetConfigDto
    {
        /// <summary>
        /// Gets or sets the configuration identifier.
        /// </summary>
        /// <value>
        /// The configuration identifier.
        /// </value>
        public int ConfigId { get; set; }
        /// <summary>
        /// Gets or sets the name of the test.
        /// </summary>
        /// <value>
        /// The name of the test.
        /// </value>
        public string TestName { get; set; }
        /// <summary>
        /// Gets or sets the low threshold.
        /// </summary>
        /// <value>
        /// The low threshold.
        /// </value>
        public float? LowThreshold { get; set; }
        /// <summary>
        /// Gets or sets the high threshold.
        /// </summary>
        /// <value>
        /// The high threshold.
        /// </value>
        public float? HighThreshold { get; set; }
        /// <summary>
        /// Gets or sets the critical threshold.
        /// </summary>
        /// <value>
        /// The critical threshold.
        /// </value>
        public float? CriticalThreshold { get; set; }
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public string Version { get; set; }
        /// <summary>
        /// Gets or sets the updated at.
        /// </summary>
        /// <value>
        /// The updated at.
        /// </value>
        public DateTime UpdatedAt { get; set; }
    }
}