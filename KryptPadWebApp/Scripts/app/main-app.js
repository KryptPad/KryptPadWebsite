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
        // Indicates whether the menu is open
        self.menuOpen = ko.observable(false);


        // Menu functionality
        self.openMenu = function () {
            self.menuOpen(!self.menuOpen());
        };

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

            // Check to see if the user is logged in
            if (!self.isSignedIn()) {
                // Go to sign in page
                window.location = '/app/signin';
                return;
            }

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
            // Hide menu
            self.menuOpen(false);
        };

        // Profiles
        self.goToProfiles = function () {
            // Go to sign in page
            window.location = '#profiles';
            // Hide menu
            self.menuOpen(false);
        };


        // Setup routes
        Sammy(function () {

            // GET: My-Account
            this.get('#my-account', function (context) {
                // Switch to template
                self.switchTemplate('my-account-template', new MyAccountViewModel());
            });

            this.get('#change-password', function (context) {
                // Switch to template
                self.switchTemplate('change-password-template', new ChangePasswordViewModel());
            });

            // GET: Profiles
            this.get('#profiles', function (context) {
                // Switch to template
                self.switchTemplate('profiles-template', new ProfilesViewModel());
            });

            // When there is no route defined
            this.get('/app', function () { this.app.runRoute('get', '#profiles') });

        }).run();

    }

    /*
     * Profiles
     */
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

    /*
     * My account
     */
    function MyAccountViewModel() {
        var self = this;
    }

    /*
     * Change password
     */
    function ChangePasswordViewModel() {
        var self = this;
        
        // Define some observables
        self.isBusy = ko.observable(false);
        self.message = ko.observable();
        
        self.currentPassword = ko.observable();
        self.newPassword = ko.observable();
        self.confirmPassword = ko.observable();
        self.success = ko.observable(false);

        self.buttonEnabled = ko.pureComputed(function () {
            // Enable sign up button when the password field is filled out properly.
            return ko.unwrap(self.currentPassword) && ko.unwrap(self.newPassword) && ko.unwrap(self.confirmPassword);
        });

        // Behaviors
        self.changePassword = function () {
            // Send all the data we need to the api to reset the password
            var postData = {
                currentPassword: self.currentPassword(),
                newPassword: self.newPassword(),
                confirmPassword: self.confirmPassword()
            };

            $.ajax({
                type: 'POST',
                url: '/api/account/change-password',
                data: postData,
                headers: app.authorizeHeader()
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
    var model = new AppViewModel();

    // Apply bindings
    ko.applyBindings(model, node);

})(window);