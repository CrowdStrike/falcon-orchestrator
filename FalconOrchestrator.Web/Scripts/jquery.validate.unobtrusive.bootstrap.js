/***************************************************
 * jQuery Unobtrusive Validation Bootstrap
 * By David Haney, Imperative Bytes LLC
 * http://www.imperativebytes.com
 ***************************************************/

(function($, window, undefined) {
    $(document).ready(function() {
        //set success: null if you don't want form elements to turn green when valid
        var classes = { groupIdentifier: ".form-group", error: "has-error", success: "has-success" };

        function onError(inputElement, message) {
            updateClasses(inputElement, classes.error, classes.success);

            inputElement.tooltip("destroy")
			    .tooltip({ html: true, title: "<b>" + message + "</b>" });
        }

        function onSuccess(inputElement) {
            updateClasses(inputElement, classes.success, classes.error);

            inputElement.tooltip("destroy");
        }

        function onValidated(errorMap, errorList) {
            if (this.settings.success) {
                $.each(this.successList, function() {
                    onSuccess($(this));
                });
            }
            
            $.each(errorList, function() {
                onError($(this.element), this.message);
            });
        }

        function updateClasses(inputElement, toAdd, toRemove) {
            var group = inputElement.closest(classes.groupIdentifier);
            if (group.length > 0) {
                group.addClass(toAdd).removeClass(toRemove);
            }
        }

        $("form").each(function() {
            var validator = $(this).data("validator");
            validator.settings.showErrors = onValidated;
        });
    });
}(jQuery, window));


