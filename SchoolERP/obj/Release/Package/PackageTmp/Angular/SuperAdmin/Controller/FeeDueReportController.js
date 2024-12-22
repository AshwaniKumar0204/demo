(function () {
    'use strict';

    angular
        .module('app')
        .controller('FeeDueReportController', FeeDueReportController);

    FeeDueReportController.$inject = ['$scope', 'superAdminService', 'Factory', 'ConstantData'];

    function FeeDueReportController($scope, superAdminService, Factory, ConstantData) {
        //$scope.checkSysytemLogin();
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
            superAdminService.getAcademicSessionList(1)
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
            superAdminService.getMonthList()
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
            superAdminService.getClassList(ConstantData.ClassStatus[1].Key)
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
            superAdminService.getSectionList(ConstantData.SectionStatus[1].Key)
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
            $scope.dataLoading = true;
            superAdminService.getFeeDueReport($scope.SectionId, $scope.MonthNo, $scope.AcademicSessionId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        console.log(response.data);
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

        $scope.getFeeDueReportExcel = function () {
            $scope.dataLoading = true;
            superAdminService.getFeeDueReportExcel($scope.SectionId, $scope.MonthNo, $scope.AcademicSessionId)
                .then(function (response) {
                    console.log(response);
                    if (response.data.Message == 'Success') {
                        toastr.success("Report Generated Successfully");
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
            $scope.TotalClassFee = 0;
            $scope.TotalTransportFee = 0;
            $scope.TotalChargeAmount = 0;
            $scope.TotalArrearAmount = 0;
            $scope.TotalFineAmount = 0;
            $scope.TotalDuesAmount = 0;
            $scope.TotalHostelCharge = 0;
            $scope.TotalAmount = 0;

            angular.forEach($scope.AdmissionList, function (admission, key) {
                $scope.TotalClassFee += admission.DueFeeDetail.ClassFee;
                $scope.TotalTransportFee += admission.DueFeeDetail.TransportFee;
                $scope.TotalChargeAmount += admission.DueFeeDetail.ChargeAmount;
                $scope.TotalArrearAmount += admission.DueFeeDetail.ArrearAmount;
                $scope.TotalFineAmount += admission.DueFeeDetail.FineAmount;
                $scope.TotalDuesAmount += admission.DueFeeDetail.PreviousDues;
                $scope.TotalHostelCharge += admission.DueFeeDetail.HostelCharge;
                $scope.TotalAmount += admission.DueFeeDetail.TotalDues;
            })
        }

        $scope.tablesToExcel = function (tables, wsnames, wbname, appname) {
            var initailPage = $scope.pageSize;
            $scope.pageSize = $scope.sizeOptions[$scope.sizeOptions.length - 1];
            $scope.IsExporting = true;
            setTimeout(function () {
                tablesToExcel(tables, wsnames, wbname, appname);
            }, 500)
            setTimeout(function () {
                $scope.IsExporting = false;
                $scope.pageSize = initailPage;
                $scope.$apply();
            }, 1000)
        }
    }
})();
