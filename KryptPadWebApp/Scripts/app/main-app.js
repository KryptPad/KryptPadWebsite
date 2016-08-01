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
        // Menu items
        self.menuOptions = ko.observable([
            {
                label: 'My account',
                url: '#my-account',
                active: ko.observable(false),
                icon: 'fa-user',
                handler: function () {
                    // Go to sign in page
                    window.location = this.url;
                    // Hide menu
                    self.menuOpen(false);
                }
            },
            {
                label: 'Profiles',
                url: '#profiles',
                active: ko.observable(false),
                icon: 'fa-files-o',
                handler: function () {
                    // Go to sign in page
                    window.location = this.url;
                    // Hide menu
                    self.menuOpen(false);
                }
            }
        ]);

        // Just vars
        self.emailHash = null;

        // Gets the user name
        self.userName = ko.pureComputed(function () {
            return app.getUserName();
        });

        // Gets the user's profile pic from gravitar
        self.profilePic = ko.pureComputed(function () {
            return 'http://www.gravatar.com/avatar/' + self.emailHash + '?d=identicon&s=200'
        });
        
        /*
         * Methods
         */
        // Menu functionality
        self.openMenu = function () {
            self.menuOpen(!self.menuOpen());
        };

        // Sign out of the system
        self.signOut = function () {
            app.logout();
        };

        // Switches the template to a new one
        self.switchTemplate = function (name, model) {

            // Check to see if the user is logged in
            if (!app.isAuthenticated()) {
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

        // Clears the active flag on the menu item
        self.clearActiveStatus = function () {
            ko.utils.arrayForEach(self.menuOptions(), function (item) {
                item.active(false);
            });
        };

        // Get account details
        self.getAccountDetails = function () {
            // Get the account details
            api.getAccountDetails().done(function (data) {
                // Set some observables
                self.emailHash = data.EmailHash;
            });
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
                // Set active
                self.clearActiveStatus();
                self.menuOptions()[0].active(true);
            });

            this.get('#my-account/change-password', function (context) {
                // Switch to template
                self.switchTemplate('change-password-template', new ChangePasswordViewModel());
            });

            // GET: Profiles
            this.get('#profiles', function (context) {
                // Switch to template
                self.switchTemplate('profiles-template', new ProfilesViewModel());
                // Set active
                self.clearActiveStatus();
                self.menuOptions()[1].active(true);
            });

            // When there is no route defined
            this.get('/app', function () { this.app.runRoute('get', '#profiles') });

        }).run();

        

        // Load the details
        self.getAccountDetails();
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
            api.getProfiles().done(function (data) {
                // Bind the profiles to the list
                $.each(data.Profiles, function () {
                    self.profiles.push(this);
                });

            }).fail(function (error) {
                app.processError(error, function (message) {
                    self.errorMessage(message);
                });
            }).always(function () {
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

        // Observables for the template
        self.template = ko.observable();
        self.templateModel = ko.observable();
        self.templateActive = ko.observable(false);

        /*
         * Methods
         */

        // Switches the template to a new one
        self.switchTemplate = function (name, model) {

            // Check to see if the user is logged in
            if (!app.isAuthenticated()) {
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

        // Setup routes
        Sammy(function () {

            //// GET: My-Account
            //this.get('#my-account/options', function (context) {
            //    // Switch to template
            //    self.switchTemplate('account-options-template', new AccountOptionsViewModel());
            //});

            this.get('#my-account/change-password', function (context) {
                // Switch to template
                self.switchTemplate('change-password-template', new ChangePasswordViewModel());
            });

            //// When there is no route defined
            //this.get('#my-account', function () { this.app.runRoute('get', '#my-account') });

        }).run();

        // Go to default template
        self.switchTemplate('account-options-template', new AccountOptionsViewModel());
    }

    // Account options
    function AccountOptionsViewModel() {
        var self = this;

        // Observables
        self.message = ko.observable();
        self.showEmailConfirmationWarning = ko.observable(false);

        /*
         * Methods
         */

        // Send an email with a confirmation link to click on
        self.sendEmailConfirmationLink = function () {
            $.ajax({
                type: 'POST',
                url: '/api/account/send-email-confirmation-link',
                headers: app.authorizeHeader()
            }).done(function (data) {
                // Set some observables
                self.message(app.createMessage(app.MSG_SUCCESS,
                    'An email was sent to your email address. Please click on the link to confirm your account.'));

            }).fail(function (error) {
                // Failed
                app.processError(error, function (message) {
                    // Show the error somewhere
                    self.message(app.createMessage(app.MSG_ERROR, message));
                });

            }).always(function () {

            });
        };

        // Get account details
        self.getAccountDetails = function () {

            api.getAccountDetails().done(function (data) {
                // Set some observables
                self.showEmailConfirmationWarning(!data.EmailConfirmed);

            }).fail(function (error) {
                // Failed
                app.processError(error, function (message) {
                    // Show the error somewhere
                    self.message(app.createMessage(app.MSG_ERROR, message));
                });

            });
        };

        // Load the details
        self.getAccountDetails();

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
            // Check if model is valid
            if (!self.isValid()) {
                return;
            }

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

        /*
         * Validation
         */

        // Errors
        self.errors = ko.validation.group(self);

        // Email
        self.currentPassword.extend({
            required: {
                message: 'Current password is required'
            }
        });

        // Password
        self.newPassword.extend({
            required: {
                message: 'New password is required'
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
    var model = new AppViewModel();

    // Apply bindings
    ko.applyBindings(model, node);

})(window);