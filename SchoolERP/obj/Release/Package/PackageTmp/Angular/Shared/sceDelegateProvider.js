(function () {
    'use strict';
    angular.
        module('app').
        config(['$sceDelegateProvider', function config($sceDelegateProvider) {
            $sceDelegateProvider.resourceUrlWhitelist([
                'self',
                'http://*./**',
                'https://rawgit.com/**',
                'http://rawgit.com/**'
            ]);
        }
        ]);
}());