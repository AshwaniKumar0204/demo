(function () {
    "use strict";

    angular.module("app").controller("AdminController", AdminController);

    AdminController.$inject = ["$scope", "adminService", "$location", "loadDataService"];

    function AdminController($scope, adminService, $location, loadDataService) {
        $scope.getUserDetail = function () {
            $scope.UserDetail = loadDataService.getUserDetail();
            if (!$scope.UserDetail)
                window.location.href = '/site/systemlogin';
        }
        $scope.getUserDetail();

        $scope.checkEditDeleteByAction = function () {
            var actionName = location.hash.substr(3);

            $scope.CanEdit = false;
            $scope.CanDelete = false;

            for (var i = 0; i < $scope.MenuList.length; i++) {
                var menuOne = $scope.MenuList[i];
                if (menuOne.ActionName == actionName) {
                    $scope.CanDelete = menuOne.CanDelete;
                    $scope.CanEdit = menuOne.CanEdit;
                    break;
                } else {
                    for (var j = 0; j < menuOne.MenuList.length; j++) {
                        var menuTwo = menuOne.MenuList[j];
                        if (menuTwo.ActionName == actionName) {
                            $scope.CanEdit = menuTwo.CanEdit;
                            $scope.CanDelete = menuTwo.CanDelete;
                            break;
                        } else {
                            for (var k = 0; k < menuTwo.MenuList.length; k++) {
                                var menuThree = menuTwo.MenuList[k];
                                if (menuThree.ActionName == actionName) {
                                    $scope.CanEdit = menuThree.CanEdit;
                                    $scope.CanDelete = menuThree.CanDelete;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        $scope.getUserMenu = function () {
            $scope.dataLoading = true;
            adminService.getUserMenu()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.MenuList = response.data.MenuList;
                        $scope.checkEditDeleteByAction();
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
                    $scope.dataLoading = false;
                })
        }
        $scope.getUserMenu();
      
        $scope.checkSysytemLogin = function () {
            if ($scope.MenuList != undefined && $scope.MenuList != null)
                $scope.checkEditDeleteByAction();
            $scope.dataLoading = true;
            adminService.checkSysytemLogin()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        if (response.data.LoginId == 0)
                            window.location.href = '/site/systemlogin';
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
                    $scope.dataLoading = false;
                })
        }
        $scope.checkSysytemLogin();
        
        $scope.logoutSystemLogin = function () {
            $scope.dataLoading = true;
            adminService.logoutSystemLogin()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        window.location.href = '/site/systemlogin';
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
                    $scope.dataLoading = false;
                })
        }

        $scope.printA4Portrait = function (id, pageSize, PageLayout) {
            sessionStorage.setItem('print-page', document.getElementById(id).innerHTML);
            sessionStorage.setItem('page_size', pageSize);
            sessionStorage.setItem('page_layout', PageLayout);
            window.open("/site/printa4portrait");
        }

        $scope.printA4Landscape = function (id) {
            sessionStorage.setItem('print-page', document.getElementById(id).innerHTML);
            window.open("/site/printA4Landscape");
        }

        $scope.PrintA4PortraitTimeTable = function (id, pageSize, PageLayout) {
            sessionStorage.setItem('print-page', document.getElementById(id).innerHTML);
            sessionStorage.setItem('page_size', pageSize);
            sessionStorage.setItem('page_layout', PageLayout);
            window.open("/site/printa4portrait");
        }

        $scope.clickM1 = function (menu1, menu2, ActionName, menuId) {
            var m1 = document.getElementById(menu1);
            var m2 = document.getElementById(menu2);
            if (m1.classList == 'ng-scope nav-item nav-item-submenu') {
                m1.className = 'ng-scope nav-item nav-item-submenu nav-item-open';
                m2.style.display = 'block';

            } else if (m1.className == 'ng-scope nav-item nav-item-submenu nav-item-open') {
                m1.className = 'ng-scope nav-item nav-item-submenu'
                m2.style.display = 'none';
            }
            //if (ActionName != null && ActionName != '' && ActionName != undefined) {
            //    $location.path(ActionName);
            //}
        }
    }
}());
