(function () {
    'use strict';

    angular
        .module('app')
        .controller('FeeDueReport2Controller', FeeDueReport2Controller);

    FeeDueReport2Controller.$inject = ['$scope', 'adminService', 'Factory', 'ConstantData'];

    function FeeDueReport2Controller($scope, adminService, Factory, ConstantData) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[3];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.ClassId = 0;
        $scope.MonthDataNo = (new Date()).getMonth() + 1;
        $scope.Year = (new Date()).getFullYear();

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

        $scope.getEnumMonthList = function () {
            $scope.dataLoading = true;
            adminService.getEnumMonthList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.MonthDataList = response.data.MonthList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getEnumMonthList();

        $scope.getMonthList = function () {
            if ($scope.ClassId == 0) {
                $scope.AcademicSessionList = [];
                $scope.AcademicSessionId = null;
                return;
            }

            $scope.dataLoading = true;
            adminService.getMonthList($scope.ClassId)
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
                        var index = { ClassId: 0, ClassName: '--Select Course--' };
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
            var index = { SectionId: 0, SectionName: '--Select Branch--' };
            $scope.SectionList.unshift(index);
            $scope.SectionId = 0;
            $scope.getAcademicSessionList();
            $scope.AcademicSessionId = null;
            $scope.getMonthList();
            $scope.MonthNo = null;
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
            if ($scope.ClassId == 0) {
                toastr.error("Course is required!!");
                return;
            }
            if ($scope.SectionId == 0) {
                toastr.error("Branch is required!!");
                return;
            }
            if ($scope.AcademicSessionId == null) {
                toastr.error("Session is required!!");
                return;
            }
            var obj = {
                SectionId: $scope.SectionId,
                ClassId: $scope.ClassId,
                AcademicSessionId: $scope.AcademicSessionId,
                UptoMonthNo: $scope.MonthDataNo,
                UpToYear: $scope.Year,
                UpToSemester: $scope.MonthNo,
            }
            $scope.dataLoading = true;
            adminService.getFeeDueReport2(obj)
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
            $scope.TotalClassFee = 0;
            $scope.TotalSchoolDiaryCharge = 0;
            $scope.TotalDuesAmount = 0;
            $scope.TotalAmount = 0;

            angular.forEach($scope.AdmissionList, function (admission, key) {
                $scope.TotalClassFee += admission.DueFeeDetail.ClassFee;
                $scope.TotalSchoolDiaryCharge += admission.DueFeeDetail.SchoolDiaryCharge;
                $scope.TotalDuesAmount += admission.DueFeeDetail.PreviousDues;
                $scope.TotalAmount += admission.DueFeeDetail.TotalDues2;
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
