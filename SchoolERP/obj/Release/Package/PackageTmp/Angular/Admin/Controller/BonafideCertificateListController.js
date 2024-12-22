(function () {
    'use strict';

    angular
        .module('app')
        .controller('BonafideCertificateListController', BonafideCertificateListController);

    BonafideCertificateListController.$inject = ['$scope', 'adminService', 'Factory', 'ConstantData'];

    function BonafideCertificateListController($scope, adminService, Factory, ConstantData) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.ClassId = 0;


        $scope.getAcademicSessionList = function () {
            $scope.dataLoading = true;
            adminService.getAcademicSessionList(ConstantData.SessionStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.AcademicSessionList = response.data.AcademicSessionList;
                        $scope.AcademicSessionId = $scope.AcademicSessionList[0].AcademicSessionId;
                        $scope.getBonafideCertificateList();
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getAcademicSessionList();

        $scope.getClassList = function () {
            $scope.dataLoading = true;
            adminService.getClassList(ConstantData.ClassStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ClassList = response.data.ClassList;
                        var index = { ClassId: 0, ClassName: '--All Classes--' };
                        $scope.ClassList.unshift(index);
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getClassList();

        $scope.changeClass = function () {
            $scope.SectionList = [];
            angular.forEach($scope.MainSectionList, function (section, key) {
                if (section.ClassId == $scope.ClassId) {
                    $scope.SectionList.push(section);
                }
            })
            var index = { SectionId: 0, SectionName: '--All Section--' };
            $scope.SectionList.unshift(index);
            $scope.SectionId = 0;
            $scope.filterList();
        }

        $scope.filterList = function () {
            $scope.BonafideCertificateList = [];
            angular.forEach($scope.MainBonafideCertificateList, function (admission, key) {
                if ($scope.ClassId == 0) {
                    $scope.BonafideCertificateList.push(admission);
                } else if ($scope.ClassId == admission.ClassId) {
                    if ($scope.SectionId == 0) {
                        $scope.BonafideCertificateList.push(admission);
                    } else if (admission.SectionId == $scope.SectionId) {
                        $scope.BonafideCertificateList.push(admission);
                    }
                }
            });
        }

        $scope.getSectionList = function () {
            $scope.dataLoading = true;
            adminService.getSectionList(ConstantData.SectionStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.MainSectionList = response.data.SectionList;
                        $scope.changeClass();
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getSectionList();

        $scope.getBonafideCertificateList = function () {
            $scope.dataLoading = true;
            adminService.getBonafideCertificateList($scope.AcademicSessionId, 0, 0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.MainBonafideCertificateList = response.data.BonafideCertificateList;
                        $scope.filterList();
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

    }
})();
