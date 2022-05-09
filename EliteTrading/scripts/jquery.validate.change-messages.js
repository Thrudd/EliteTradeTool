// Change the default message of jquery validation
jQuery.extend(jQuery.validator.messages, {
    required: "Required",
    remote: "Please fix this field",
    email: "Please enter a valid email address",
    url: "Invalid URL",
    date: "Invalid date",
    dateISO: "Invalid date (ISO)",
    number: "Invalid number",
    digits: "Digits only",
    creditcard: "Invalid credit card number",
    equalTo: "Doesn't match",
    accept: "Invalid file extension",
    maxlength: jQuery.validator.format("No more than {0} characters"),
    minlength: jQuery.validator.format("Must be at least {0} characters"),
    rangelength: jQuery.validator.format("Must be between {0} and {1} characters long"),
    range: jQuery.validator.format("Must be between {0} and {1}"),
    max: jQuery.validator.format("Must be less than or equal to {0}"),
    min: jQuery.validator.format("Must be greater than or equal to {0}")
});