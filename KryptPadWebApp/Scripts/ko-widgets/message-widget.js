ko.components.register('message-widget', {
    viewModel: function (params) {
        var self = this;

        self.message = params.message;
        
        self.messageType = ko.computed(function () {
            var msgObj = ko.unwrap(self.message);

            if (msgObj) {
                return msgObj.type;
            }
        });

        self.messageText = ko.computed(function () {
            var msgObj = ko.unwrap(self.message);

            if (msgObj) {
                return msgObj.message;
            }
        });

        self.isError = ko.computed(function () {
            return (self.messageType() === 0);
        });

        self.isSuccess = ko.computed(function () {
            return (self.messageType() === 1);
        });
    },
    template: { fromUrl: 'message-widget.html' }
});
