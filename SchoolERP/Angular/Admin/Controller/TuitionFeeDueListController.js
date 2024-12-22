(function () {
    'use strict';

    angular
        .module('app')
        .controller('TuitionFeeDueListController', TuitionFeeDueListController);

    TuitionFeeDueListController.$inject = ['$scope', 'adminService', 'Factory', 'ConstantData'];
    function TuitionFeeDueListController($scope, adminService, Factory, ConstantData) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[3];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.ClassId = 0;
        $scope.MonthNo = (new Date()).getMonth() + 1;

        $scope.getAcademicSessionList = function () {
            $scope.dataLoading = true;
            adminService.getAcademicSessionList(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.AcademicSessionList = response.data.AcademicSessionList;
                        $scope.AcademicSessionId = $scope.AcademicSessionList[$scope.AcademicSessionList.length - 1].AcademicSessionId;
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

        $scope.getMonthList = function () {
            $scope.dataLoading = true;
            adminService.getMonthList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.MonthList = response.data.MonthList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getMonthList();

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

        $scope.getFeeDueReport = function () {
            if ($scope.AcademicSessionId == 0) {
                toastr.error("Session is required!!");
                return;
            }
            if ($scope.ClassId == 0) {
                toastr.error("Class is required!!");
                return;
            }
            if ($scope.SectionId == 0) {
                toastr.error("Section is required!!");
                return;
            }
            $scope.dataLoading = true;
            adminService.getTuitionFeeDueList($scope.SectionId, $scope.MonthNo, $scope.AcademicSessionId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.AdmissionList = response.data.AdmissionList;
                        $scope.calculateTotal();
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.calculateTotal = function () {
            $scope.TotalTuitionFee = 0;
            $scope.TotalPreDues = 0;
            $scope.TotalTransportFee = 0;
            $scope.TotalAnnualFee = 0;
            angular.forEach($scope.AdmissionList, function (admission, key) {
                $scope.TotalTuitionFee += admission.DueFeeDetail.TuitionFee;
                $scope.TotalPreDues += admission.DueFeeDetail.PreviousDues;
                $scope.TotalTransportFee += admission.DueFeeDetail.TransportFee;
                $scope.TotalAnnualFee += admission.DueFeeDetail.AnnualFee;
            })
        }
    }
})();
