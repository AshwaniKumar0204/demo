(function () {
    'use strict';

    angular
        .module('app')
        .controller('DailyConcessionReportStudentwiseController', DailyConcessionReportStudentwiseController);

    DailyConcessionReportStudentwiseController.$inject = ['$scope', 'adminService', 'Factory', 'ConstantData', 'loadDataService'];

    function DailyConcessionReportStudentwiseController($scope, adminService, Factory, ConstantData, loadDataService) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[0];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.PaymentModeList = ConstantData.PaymentModeAll;
        $scope.DCR = {};
        $scope.DCR.PaymentMode = null;
        $scope.DCR.LoginId = 0;
        $scope.DCR.FromDate = new Date();
        $scope.DCR.ToDate = new Date();
        $scope.DailyConcessionReport = [];
        $scope.DailyConcessionReport.TableHeaderList = [];


        $scope.$watch('formDCR.$valid', function (value) {
            $scope.IsFormDCRValid = value;
        })

        $scope.getSystemLoginList = function () {
            $scope.dataLoading = true;
            adminService.getSystemLoginList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.SystemLoginList = response.data.SystemLoginList;
                        var index = { LoginId: 0, UserName: '--All Users--' };
                        $scope.SystemLoginList.unshift(index);
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getSystemLoginList();

        $scope.getFeeHeadList = function () {
            $scope.dataLoading = true;
            adminService.getFeeHeadList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.FeeHeadList = response.data.FeeHeadList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getFeeHeadList();

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

        $scope.getAcademicSessionList = function () {
            if ($scope.ClassId == 0) {
                $scope.AcademicSessionList = [];
                $scope.AcademicSessionId = null;
                return;
            }
            var obj = {
                ClassId: $scope.ClassId
            }
            $scope.dataLoading = true;
            adminService.getAcademicSessionList(obj)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.MainAcademicSessionList = response.data.AcademicSessionList;
                        $scope.AcademicSessionList = response.data.AcademicSessionList;
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
                if (section.ClassId == $scope.DCR.ClassId) {
                    $scope.SectionList.push(section);
                }
            })

            $scope.AcademicSessionList = [];
            $scope.AcademicSessionId = null;
            if ($scope.DCR.ClassId == null)
                $scope.AcademicSessionList = $scope.MainAcademicSessionList;
            else
                angular.forEach($scope.MainAcademicSessionList, function (a1) {
                    if (a1.ClassId == $scope.DCR.ClassId)
                        $scope.AcademicSessionList.push(a1);
                });
        }

        $scope.getDailyConcessionReport = function () {
            $scope.IsSubmitted = true;
            if (!$scope.IsFormDCRValid) {
                toastr.error("From Date and To Date is required!!");
                return;
            }
            var DCRRequest = {
                FromDate: loadDataService.getDateTime($scope.DCR.FromDate),
                ToDate: loadDataService.getDateTime($scope.DCR.ToDate),
                FeeHeadList: $scope.FeeHeadList,
                LoginId: $scope.DCR.LoginId,
                PaymentMode: $scope.DCR.PaymentMode,
                ClassId: $scope.DCR.ClassId,
                SectionId: $scope.DCR.SectionId,
            }
            $scope.dataLoading = true;
            adminService.getDailyConcessionReportStudentwise(DCRRequest)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.DailyConcessionReport = response.data.DailyConcessionReport;
                        $scope.DailyConcessionReport.MainTableItemList = response.data.DailyConcessionReport.TableItemList;
                        $scope.filterList();
                        if ($scope.DailyConcessionReport.TableItemList.length == 0) {
                            toastr.error("No fee concession is found!!");
                        }

                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.filterList = function () {
            $scope.DailyConcessionReport.TableItemList = [];
            if ($scope.SessionName != null && $scope.SessionName != undefined) {
                angular.forEach($scope.DailyConcessionReport.MainTableItemList, function (d1, k1) {
                    if (d1.DynamicList[5].Value == $scope.SessionName) {
                        $scope.DailyConcessionReport.TableItemList.push(d1)
                        //for (var i = 6; i < d1.DynamicList.length; i++) {

                        //}
                    }
                });
            } else {
                $scope.DailyConcessionReport.TableItemList = $scope.DailyConcessionReport.MainTableItemList;
            }
        }

        $scope.getDailyConcessionReport2 = function () {
            $scope.IsSubmitted = true;
            if (!$scope.IsFormDCRValid) {
                toastr.error("From Date and To Date is required!!");
                return;
            }
            var DCRRequest = {
                FromDate: loadDataService.getDateTime($scope.DCR.FromDate),
                ToDate: loadDataService.getDateTime($scope.DCR.ToDate),
                FeeHeadList: $scope.FeeHeadList,
                LoginId: $scope.DCR.LoginId,
                PaymentMode: $scope.DCR.PaymentMode,
                ClassId: $scope.DCR.ClassId,
                SectionId: $scope.DCR.SectionId,
            }
            $scope.dataLoading = true;
            adminService.getDailyConcessionReportStudentwise2(DCRRequest)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.DailyConcessionReport = response.data.DailyConcessionReport;
                        if ($scope.DailyConcessionReport.TableItemList.length == 0) {
                            toastr.error("No fee concession is found!!");
                        }

                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.tablesToExcel = function (tables, wsnames, wbname, appname) {
            $scope.dataLoading = true;
            var initailPage = $scope.pageSize;
            $scope.pageSize = $scope.sizeOptions[$scope.sizeOptions.length - 1];
            $scope.IsExporting = true;
            setTimeout(function () {
                tablesToExcel(tables, wsnames, wbname, appname);
            }, 500)
            setTimeout(function () {
                $scope.IsExporting = false;
                $scope.pageSize = initailPage;
                $scope.dataLoading = false;
                $scope.$apply();
            }, 1000)
        }
    }
})();
