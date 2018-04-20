(function () {
    'use strict';
    var serviceId = 'messageFactory';

    angular.module('MessagesApp').factory(serviceId,
        ['$http', messageFactory]);

    function messageFactory($http) {

        function getMessages() {
            return $http.get('/api/Message');
        }

        var service = {
            getMessages: getMessages
        };

        return service;
    }
})();