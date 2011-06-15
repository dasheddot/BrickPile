﻿using System.ComponentModel.DataAnnotations;

namespace BrickPile.Sample.Models {
    /// <summary>
    /// Represents the base editoral, contains common properties that all editorial models have
    /// </summary>
    public class BaseEditorial : BasePageModel {
        /// <summary>
        /// Gets or sets the heading.
        /// </summary>
        /// <value>
        /// The heading.
        /// </value>
        public virtual string Heading { get; set; }
        /// <summary>
        /// Gets or sets the main intro.
        /// </summary>
        /// <value>
        /// The main intro.
        /// </value>
        [DataType(DataType.MultilineText)]
        public virtual string MainIntro { get; set; }
        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>
        /// The image.
        /// </value>
        [DataType(DataType.ImageUrl)]
        public string Image { get; set; }
        /// <summary>
        /// Gets or sets the main body.
        /// </summary>
        /// <value>
        /// The main body.
        /// </value>
        [DataType(DataType.Html)]
        public virtual string MainBody { get; set; }
    }
}