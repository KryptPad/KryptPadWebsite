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
        // Enable or disable template rendering
        self.templateActive = ko.observable(false);

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
            // Turn off rendering
            self.templateActive(false);
            // Out put some console stuff
            console.log("Switching template to " + name);

            // Set new view model
            self.templateModel(model);

            // Trigger rebind of template
            self.template(name);
            
            // Turn on rendering
            self.templateActive(true);
        };

        /*
         * Navigation methods
         */
        self.goToMyAccount = function () {
            // Go to sign in page
            window.location = '#my-account';
        };

        // Setup routes
        Sammy(function () {

            // GET: My-Account
            this.get('#my-account', function (context) {
                // Check to see if we are authenticated
                if (self.isSignedIn()) {
                    // Switch to template
                    self.switchTemplate('my-account-template', new MyAccountViewModel());
                } else {
                    // Go to sign in page
                    window.location = '/app/signin';
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

    // My Account
    function MyAccountViewModel() {
        var self = this;
    }

    // Create model
    var model = new AppViewModel();

    // Apply bindings
    ko.applyBindings(model, node);

})(window);