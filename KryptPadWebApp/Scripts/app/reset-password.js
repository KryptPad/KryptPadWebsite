(function (global) {

    // Get main app container
    var node = document.getElementById('reset-password');

    function ViewModel() {
        var self = this;

        // Store the data we got passed
        self.code = app.request('code');

        // Define some observables
        self.isBusy = ko.observable(false);
        self.message = ko.observable();
        self.email = ko.observable();
        self.password = ko.observable();
        self.confirmPassword = ko.observable();
        self.success = ko.observable(false);

        self.resetEnabled = ko.pureComputed(function () {
            var pw = ko.unwrap(self.password);
            var cp = ko.unwrap(self.confirmPassword);
            // Enable sign up button when the email, password and confirm password fields are filled out
            // properly.
            return ko.unwrap(self.email) && pw && pw === cp;
        });

        // Behaviors
        self.reset = function () {
            // Send all the data we need to the api to reset the password
            var postData = {
                email: self.email(),
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
    }

    // Create model
    var model = new ViewModel();

    // Apply bindings
    ko.applyBindings(model, node);

})(window);