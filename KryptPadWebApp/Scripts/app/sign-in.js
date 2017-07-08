(function (global) {

    // Get main app container
    var node = document.getElementById('sign-in');

    // App view model
    function ViewModel() {
        var self = this;
        debugger
        self.username = ko.observable();
        self.password = ko.observable();
        self.isBusy = ko.observable(false);
        self.message = ko.observable();

        /*
         * Methods
         */

        // Log in
        self.login = function () {
            // Check if model is valid
            if (!self.isValid()) {
                return;
            }

            // Set busy state
            self.isBusy(true);
            // Login
            api.login(self.username(), self.password()).done(function (data) {
                // Cache the access token in session storage.
                app.setToken(data);

                // Login successful, submit form for route handling. The route
                // will be picked up in the main-app.js file, and it will load
                // the profiles view
                $('#login-form').submit();

            }).fail(function (error) {
                // Set busy state to false
                self.isBusy(false);

                app.processError(error, function (message) {
                    // Show the error somewhere
                    self.message(app.createMessage(app.MSG_ERROR, message));
                });

            });
        }

        /*
         * Validation
         */

        // Errors
        self.errors = ko.validation.group(self);

        // Email
        self.username.extend({
            required: {
                message: 'Email is required'
            }
        });

        // Password
        self.password.extend({
            required: {
                message: 'Password is required'
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