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
        self.templateModel = ko.observable();
        // Gets the user name
        self.userName = ko.pureComputed(function () {
            return app.getUserName();
        });
        // Gets the user's profile pic from gravitar
        self.profilePic = ko.pureComputed(function () {

            var hash = md5(self.userName());
            return 'http://www.gravatar.com/avatar/' + hash + '?d=identicon&s=200'

        });
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

        // Sign out of the system
        self.signOut = function () {
            app.logout();
        };

        // Switches the template to a new one
        self.switchTemplate = function (name, model) {
            // Remove existing template
            var oldTemplate = self.template();
            self.template(null);
            delete oldTemplate;

            // Set new view model
            self.templateModel(model);

            // Trigger rebind of template
            self.template(name);
        };

        // Setup routes
        Sammy(function () {

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
                    //self.templateOptions(model);
                    // Trigger rebind of template
                    //self.template('reset-password-template');
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
                    //self.templateOptions(model);
                    // Trigger rebind of template
                    //self.template('confirm-email-template');
                }
                else {
                    // Show some error or go back to login

                }
            });

            // GET: Profiles
            this.get('#profiles', function (context) {
                // Check to see if we are authenticated
                if (self.isSignedIn()) {
                    // Switch to template
                    self.switchTemplate('profiles-template', new ProfilesViewModel());
                } else {
                    // Go to sign in page
                    window.location = '/app/signin';
                }
            });

            // When there is no route defined
            this.get('/app', function () { this.app.runRoute('get', '#profiles') });

        }).run();

    }

    function ProfilesViewModel() {
        var self = this;

        self.profiles = ko.observableArray([]);
        self.isBusy = ko.observable(false);
        self.errorMessage = ko.observable();

        self.getProfiles = function () {

            // Set busy state
            self.isBusy(true);

            // Authorize the request
            $.ajax({
                type: 'GET',
                url: '/api/profiles',
                headers: app.authorizeHeader()
            }).done(function (data) {
                // Bind the profiles to the list
                $.each(data.Profiles, function () {
                    self.profiles.push(this);
                });

            }).fail(function (error) {
                app.processError(error, function (message) {
                    self.errorMessage(message);
                });
            }).complete(function () {
                // Set busy state
                self.isBusy(false);
            });
        }

        // Load the profiles
        self.getProfiles();
    }

    // Create model
    var model = new AppViewModel();

    // Apply bindings
    ko.applyBindings(model, node);

})(window);