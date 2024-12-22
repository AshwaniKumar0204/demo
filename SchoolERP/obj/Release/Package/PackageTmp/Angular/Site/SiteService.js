(function () {
    "use strict";

    angular.module("app")
        .service("siteService", siteService);

    siteService.$inject = ["$http"];

    function siteService($http) {
        this.systemLogin = function (UserId, Password) {
            return $http.get("/api/login/systemlogin", { params: { UserId: UserId, Password: Password } });
        }

        this.setUserDetail = function (UserDetail) {
            if (typeof (Storage) != null && typeof (Storage) != undefined) {
                localStorage.setItem('UserDetail', JSON.stringify(UserDetail));
                return true;
            } else {
                return false;
            }
        }
    }
}());