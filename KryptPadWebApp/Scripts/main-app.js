(function (global) {
    
    // Get main app container
    var node = document.getElementById('app');

    // App view model
    function AppViewModel() {
        var self = this;

        // Create some observables for our model
        self.test = ko.observable('This is a test');
        self.template = ko.observable();

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

            // GET: Forgot-Password
            this.get('#forgot-password', function (context) {
                // Trigger rebind of template
                self.template('forgot-password-template');
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
            this.get('', function () { this.app.runRoute('get', '#profiles') });

        }).run();
    }

    // Create model
    var model = new AppViewModel();

    // Apply bindings
    ko.applyBindings(model, node);

})(window);