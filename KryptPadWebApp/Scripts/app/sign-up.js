function Signup (options) {
    
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

            // Create an account
            api.register(
                self.email(),
                self.password(),
                self.confirmPassword()
            ).done(function (data) {
                
                // Success, log into account
                api.signin(self.email(), self.password(), options.requestUrl, options.antiForgeryToken).done(function (data) {
                    
                    // Login successful, submit form to go to select profile page
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
                    message: api.strings.emailRequired
                }
            })
            .extend({ email: true });

        // Password
        self.password.extend({
            required: {
                message: api.strings.passwordRequired
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
    ko.applyBindings(model, options.node);

}