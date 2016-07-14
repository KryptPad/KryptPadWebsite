(function (global) {

    // Get main app container
    var node = document.getElementById('reset-password');

    function ViewModel() {
        var self = this;

        // Store the data we got passed
        self.code = app.request('code');
        self.userId = app.request('userId');

        // Define some observables
        self.isBusy = ko.observable(false);
        self.message = ko.observable();

        self.password = ko.observable();
        self.confirmPassword = ko.observable();
        self.success = ko.observable(false);

        // Behaviors
        self.reset = function () {
            // Check if model is valid
            if (!self.isValid()) {
                return;
            }

            // Send all the data we need to the api to reset the password
            var postData = {
                userId: ko.unwrap(self.userId),
                code: ko.unwrap(self.code),
                password: self.password(),
                confirmPassword: self.confirmPassword()
            };

            $.ajax({
                type: 'POST',
                url: '/api/account/reset-password',
                data: postData
            }).done(function (data) {
                // Success
                self.message(app.createMessage(app.MSG_SUCCESS, "Your password has been changed."));
                // Set flag
                self.success(true);

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

        // Password
        self.password.extend({
            required: {
                message: 'Password is required'
            }
        });

        // Confirm password
        self.confirmPassword.extend({
            equal: {
                params: self.password,
                message: 'Passwords don\'t match'
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
        
    // Create model
    var model = new ViewModel();

    // Apply bindings
    ko.applyBindings(model, node);

})(window);