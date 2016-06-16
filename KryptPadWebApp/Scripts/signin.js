(function (global) {

    // Get main app container
    var node = document.getElementById('signin');

    // App view model
    function ViewModel() {
        var self = this;
        debugger
        self.username = ko.observable();
        self.password = ko.observable();
        self.isBusy = ko.observable(false);
        self.message = ko.observable();

        self.signInEnabled = ko.pureComputed(function () {
            // Sign in button is only enabled when email and password are filled out
            return ko.unwrap(self.username) && ko.unwrap(self.password);
        });

        // Log in
        self.login = function () {

            // Set busy state
            self.isBusy(true);
            // Login
            app.login(self.username(), self.password()).done(function (data) {
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
    }

    // Create model
    var model = new ViewModel();

    // Apply bindings
    ko.applyBindings(model, node);

})(window);