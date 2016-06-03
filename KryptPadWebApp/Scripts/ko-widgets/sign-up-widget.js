ko.components.register('sign-up-widget', {
    viewModel: function (params) {
        var self = this;

        self.isBusy = ko.observable(false);
        self.message = ko.observable();

        self.email = ko.observable();
        self.password = ko.observable();
        self.confirmPassword = ko.observable();

        self.signUpEnabled = ko.pureComputed(function () {
            var pw = ko.unwrap(self.password);
            var cp = ko.unwrap(self.confirmPassword);
            // Enable sign up button when the email, password and confirm password fields are filled out
            // properly.
            return ko.unwrap(self.email) && pw && pw === cp;
        });

        // Behaviors
        self.signup = function () {
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
        
    },
    template: { fromUrl: 'sign-up-widget.html' }
});
