(function (global) {

    // Get main app container
    var node = document.getElementById('forgot-password');

    // App view model
    function ViewModel() {
        var self = this;

        self.email = ko.observable();
        self.isBusy = ko.observable(false);
        self.message = ko.observable();
        self.linkSent = ko.observable(false);

        // User forgot password
        self.sendLink = function () {
            // Check if model is valid
            if (!self.isValid()) {
                return;
            }

            // Set busy state
            self.isBusy(true);

            var postData = {
                email: self.email()
            };

            $.ajax({
                type: 'POST',
                url: '/api/account/forgot-password',
                data: postData
            }).done(function (data) {
                // Success
                self.message(app.createMessage(app.MSG_SUCCESS, "If the email address you provided is associated to your account, you should receive an email shortly with instructions on how to change your password."));

                // Set the flag
                self.linkSent(true);

            }).fail(function (error) {
                // Failed
                app.processError(error, function (message) {
                    // Show the error somewhere
                    self.message(app.createMessage(app.MSG_ERROR, message));
                });

            }).always(function () {
                // Set busy state
                self.isBusy(false);

            });
        };

        /*
         * Validation
         */

        // Errors
        self.errors = ko.validation.group(self);

        // Email
        self.email.extend({
            required: {
                message: 'Email is required'
            }
        });

        // Show errors in our model
        self.isValid = function () {
            if (self.errors().length) {
                self.errors.showAllMessages();
                return false
            }

            return true;
        };

    }

    // Initialize validation
    ko.validation.init({
        registerExtenders: true,
        messagesOnModified: true,
        insertMessages: false,
        parseInputAttributes: true
    }, true);

    // Create model
    var model = new ViewModel();

    // Apply bindings
    ko.applyBindings(model, node);

})(window);