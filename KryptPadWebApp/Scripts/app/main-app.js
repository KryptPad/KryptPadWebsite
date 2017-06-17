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
            return 'http://www.gravatar.com/avatar/' + self.emailHash + '?d=identicon&s=200';
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

            // GET: Items
            this.get('#profiles/:id', function (context) {
                var id = this.params['id'];
                // Switch to template
                self.switchTemplate('items-template', new ItemsViewModel(id));
                // Set active
                self.clearActiveStatus();
                self.menuOptions()[1].active(true);
            });

            // When there is no route defined
            this.get('/app', function () { this.app.runRoute('get', '#profiles'); });

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
        self.message = ko.observable();
        self.profileMessage = ko.observable();
        self.selectedProfile = ko.observable();
        self.passphrase = ko.observable();
        self.passphraseHasFocus = ko.observable();
        self.passphraseModalOpen = ko.observable();

        /*
         * Gets the profiles for the user's account
         */
        self.getProfiles = function () {

            // Set busy state
            self.isBusy(true);

            // Get the profiles
            api.getProfiles().done(function (data) {
                // Bind the profiles to the list
                $.each(data.Profiles, function () {
                    self.profiles.push(this);
                });

            }).fail(function (error) {
                // Handle the error
                app.processError(error, function (message) {
                    self.message(app.createMessage(app.MSG_ERROR, message));
                });

            }).always(function () {
                // Set busy state
                self.isBusy(false);
            });
        };

        /*
         * Selects a profile and goes to the item page
         */
        self.enterProfile = function () {
            var profile = ko.unwrap(self.selectedProfile);
            if (!profile) {
                return;
            }

            // Call api to validate the entered passphrase
            // If successful, store the passphrase for future api request
            // Set this profile as our main context and show the items page
            api.loadProfile(profile.Id, ko.unwrap(self.passphrase)).done(function (data) {
                // Hide the modal
                self.passphraseModalOpen(false);
                // Store the passphrase we used in session storage
                app.setPassphrase(ko.unwrap(self.passphrase));
                // Clear message
                self.profileMessage(null);
                // Go to the items page
                //window.location = '#profiles/' + data.Profiles[0].Id;

            }).fail(function (error) {
                // Handle the error
                if (error.status === 401) {
                    // Wrong passphrase
                    self.profileMessage(app.createMessage(app.MSG_ERROR, "The passphrase you entered was incorrect."));
                    
                } else {
                    // Unknown error
                    self.profileMessage(app.createMessage(app.MSG_ERROR, "An unknown error occurred."));

                }
                
                // Clear passphrase
                self.passphrase(null);
                // Set focus back to passphrase
                self.passphraseHasFocus(true);

            }).always(function () {
                // Set busy state
                self.isBusy(false);
            });

            
        };

        /*
         * Sets the selected profile
         */
        self.providePassphrase = function () {
            // Set the selected profile
            self.selectedProfile(this);
        };

        // Load the profiles
        self.getProfiles();
    }

    /*
     * Items
     */
    function ItemsViewModel(profileId, passphrase) {
        var self = this;

        self.categories = ko.observableArray([]);
        self.isBusy = ko.observable(false);
        self.message = ko.observable();

        /*
         * Gets the categories and items for the specified profile
         */
        self.getCategories = function () {

            // Set busy state
            self.isBusy(true);
            
            // Get the categories with items
            api.getItems(profileId).done(function (data) {
                //ko.utils.arrayPushAll(self.items, data);
                $.each(data.Categories, function () {
                    self.categories.push(this);
                });
            }).fail(function (error) {
                // Handle the error
                app.processError(error, function (message) {
                    // Show the error somewhere
                    self.message(app.createMessage(app.MSG_ERROR, message));
                });
            }).always(function () {
                // Set busy state
                self.isBusy(false);
            });
        };

        // Load the categories with items
        self.getCategories();
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

            // Send confirm email
            api.sendEmailConfirmationLink().done(function (data) {
                // Set some observables
                self.message(app.createMessage(app.MSG_SUCCESS,
                    'An email was sent to your email address. Please click on the link to confirm your account.'));

            }).fail(function (error) {
                // Failed
                app.processError(error, function (message) {
                    // Show the error somewhere
                    self.message(app.createMessage(app.MSG_ERROR, message));
                });

            });
        };

        // Get account details
        self.getAccountDetails = function () {

            // Get account details
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

            // Change the user's password
            api.changePassword(
                self.currentPassword(),
                self.newPassword(),
                self.confirmPassword()
            ).done(function (data) {
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
                return false;
            }

            return true;
        };
    }

    // Create model
    var model = new AppViewModel();

    // Apply bindings
    ko.applyBindings(model, node);

})(window);