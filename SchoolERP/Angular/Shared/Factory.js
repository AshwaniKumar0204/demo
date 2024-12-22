(function () {
    'use strict';

    angular
        .module('app')
        .factory('Factory', Factory);

    Factory.$inject = [];

    function Factory() {
        return {
            SizeOptions: [10, 20, 50, 100, 1000,2000,5000,10000,20000,50000,100000]
        };
    }
})();