(function (global) {
    
    // Get main app container
    var node = document.getElementById('app');

    // App view model
    function AppViewModel() {
        var self = this;

        // Create some observables for our model

        // Store the name of the current view in this observable
        self.template = ko.observable();
        // Pass in options to our template with this observable
        self.templateOptions = ko.observable();

        // Behaviors

        // Go to sign in view if the user is not already authenticated
        self.isSignedIn = function () {
            // Check to see if we are authenticated
            if (!app.isAuthenticated()) {
                global.location.hash = 'login';
                // Not signed in
                return false;
            }

            // Is signed in
            return true;
        };

        // Setup routes
        Sammy(function () {

            // GET: Login
            this.get('#login', function (context) {
                // Trigger rebind of template
                self.template('login-template');
            });

            // POST: Login
            this.post('#login', function (context) {    
                // Login was successfull, refresh
                global.location = '/app';
            });

            // POST: Signup
            this.post('#signup', function (context) {
                // Sign up was successfull, refresh
                global.location = '/app';
            });

            // GET: Forgot-Password
            this.get('#forgot-password', function (context) {
                // Trigger rebind of template
                self.template('forgot-password-template');
            });

            // GET: Reset-Password
            this.get('#reset-password', function (context) {
                var userId = this.params.userId;
                var code = this.params.code;
                
                // Make sure we have a user id and code
                if (userId && code) {
                    // Create model for reset password
                    var model = {
                        code: code
                    };

                    // Set the options
                    self.templateOptions(model);
                    // Trigger rebind of template
                    self.template('reset-password-template');
                }
                else {
                    // Show some error or go back to login

                }
            });

            // GET: Confirm-Email
            this.get('#confirm-email', function (context) {
                var userId = this.params.userId;
                var code = this.params.code;

                // Make sure we have a user id and code
                if (userId && code) {
                    // Create model for reset password
                    var model = {
                        userId: userId,
                        code: code
                    };

                    // Set the options
                    self.templateOptions(model);
                    // Trigger rebind of template
                    self.template('confirm-email-template');
                }
                else {
                    // Show some error or go back to login

                }
            });

            // GET: Profiles
            this.get('#profiles', function (context) {
                // Check to see if we are authenticated
                if (self.isSignedIn()) {
                    // Trigger rebind of template
                    self.template('profiles-template');
                }
            });

            // When there is no route defined
            this.get('/app', function () { this.app.runRoute('get', '#profiles') });

        }).run();
    }

    // Create model
    var model = new AppViewModel();

    // Apply bindings
    ko.applyBindings(model, node);

})(window);