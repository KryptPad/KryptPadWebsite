(function (global) {

    // Get main app container
    var node = document.getElementById('signup');

    // App view model
    function ViewModel() {
        var self = this;

        self.isBusy = ko.observable(false);
        self.message = ko.observable();

        self.email = ko.observable();
        self.password = ko.observable();
        self.confirmPassword = ko.observable();

        // Behaviors
        self.signup = function () {
            // Check if model is valid
            if (!self.isValid()) {
                return;
            }

            // Set busy state
            self.isBusy(true);

            var postData = {
                email: self.email(),
                password: self.password(),
                confirmPassword: self.confirmPassword()
            };

            $.ajax({
                type: 'POST',
                url: '/api/account/register',
                data: postData
            }).done(function (data) {
                // Success, log into account
                app.login(self.email(), self.password()).done(function (data) {
                    // Cache the access token in session storage.
                    app.setToken(data);

                    // Login successful, submit form for route handling. The route
                    // will be picked up in the main-app.js file, and it will load
                    // the profiles view
                    $('#sign-up-form').submit();

                }).fail(function (error) {
                    // Process the error that occurred
                    app.processError(error, function (message) {
                        // Show the error somewhere
                        self.message(app.createMessage(app.MSG_ERROR, message));
                    });

                }).always(function () {
                    // Set busy state
                    self.isBusy(false);
                });

            }).fail(function (error) {
                // Set busy state
                self.isBusy(false);
                // Failed
                app.processError(error, function (message) {
                    // Show the error somewhere
                    self.message(app.createMessage(app.MSG_ERROR, message));
                });

            });
        };

        /*
         * Validation
         */

        // Errors
        self.errors = ko.validation.group(self);

        // Email
        self.email
            .extend({
                required: {
                    message: 'Email is required'
                }
            })
            .extend({ email: true });

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