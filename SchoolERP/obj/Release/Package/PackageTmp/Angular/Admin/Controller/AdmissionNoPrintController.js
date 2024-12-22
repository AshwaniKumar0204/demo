(function () {
    'use strict';

    angular
        .module('app')
        .controller('AdmissionNoPrintController', AdmissionNoPrintController);

    AdmissionNoPrintController.$inject = ['$scope', 'adminService', 'Factory', 'ConstantData', '$location'];

    function AdmissionNoPrintController($scope, adminService, Factory, ConstantData, $location) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[0];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.NoOfQuantity = 1;
        $scope.ClassId = null;
        $scope.AcademicSessionId = null;
        $scope.getAcademicSessionList = function () {

            var obj = {
                ClassId: $scope.ClassId
            }
            //alert($scope.ClassId);
            $scope.dataLoading = true;
            adminService.getAcademicSessionList(obj)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.AcademicSessionList = response.data.AcademicSessionList;
                        $scope.AcademicSessionId = $scope.AcademicSessionList[0].AcademicSessionId;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
            
        }
        //$scope.getAcademicSessionList();

        $scope.getClassList = function () {
            $scope.dataLoading = true;
            adminService.getClassList(ConstantData.ClassStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ClassList = response.data.ClassList;
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
            
            $scope.getAcademicSessionList();
            $scope.SectionList = [];
            angular.forEach($scope.MainSectionList, function (section, key) {
                if (section.ClassId == $scope.ClassId) {
                    $scope.SectionList.push(section);
                }
            })
            //var index = { SectionId: null, SectionName: '--Select Section--' };
            //$scope.SectionList.unshift(index);
        }

        $scope.getSectionList = function () {
            $scope.dataLoading = true;
            adminService.getSectionList(ConstantData.SectionStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.MainSectionList = response.data.SectionList;
                        //$scope.changeClass();
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

        $scope.CheckedAll = function () {
            angular.forEach($scope.AdmissionList, function (a1, key1) {
                if ($scope.IsCheckedAll)
                    a1.IsChecked = true;
                else
                    a1.IsChecked = false;
            })
        }

        $scope.getAdmissionList = function () {
            $scope.dataLoading = true;
            adminService.getAdmissionListForLibrary($scope.AcademicSessionId, $scope.ClassNo, $scope.SectionId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.AdmissionList = response.data.AdmissionList;
                        $scope.IsCheckedAll = true;
                        $scope.CheckedAll();
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.printReport = function () {
            if ($scope.NoOfQuantity == null || $scope.NoOfQuantity == undefined) {
                toastr.error("Quantity is required!!");
                return;
            }
            var admissionList = [];
            angular.forEach($scope.AdmissionList, function (a1, key1) {
                if (a1.IsChecked) {
                    var obj = { id: a1.AdmissionNo, NoOfQuantity: $scope.NoOfQuantity };
                    admissionList.push(obj);
                }
            })
            if (admissionList.length == 0) {
                toastr.error("No Student is Selcted!!");
                return;
            }

            $scope.dataLoading = true;
            adminService.setSessionForPrintBarcode(admissionList)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        window.open('/print/PrintBarCodeAdmissionNos');
                        $scope.AccessionList = [];
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.printIndividualReport = function (a1) {
            var admissionList = [];
            var obj = { id: a1.AdmissionNo, NoOfQuantity: 2 };
            admissionList.push(obj);

            $scope.dataLoading = true;
            adminService.setSessionForPrintBarcode(admissionList)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        window.open('/print/PrintBarCodeAdmissionNos');
                        $scope.AccessionList = [];
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
