(function () {
    'use strict';

    angular
        .module('app')
        .controller('AllowMarksheetDownloadController', AllowMarksheetDownloadController);

    AllowMarksheetDownloadController.$inject = ['$scope', 'adminService', 'Factory', 'ConstantData', '$location'];

    function AllowMarksheetDownloadController($scope, adminService, Factory, ConstantData, $location) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[0];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };

        $scope.ExamTermList = ConstantData.ExamTerm;
        $scope.ExamTerm = null;

        $scope.$watch('formMarksheet.$valid', function (value) {
            $scope.isFormMarksheetValid = value;
        })
        
        $scope.getAllowMarksheetList = function () {
            $scope.IsSubmitted = true;
            if ($scope.isFormMarksheetValid) {
                $scope.dataLoading = true;
                adminService.getAllowMarksheetList($scope.AcademicSessionId, $scope.SectionId)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            $scope.MarksheetList = response.data.MarksheetList;
                            if ($scope.MarksheetList.length == 0) {
                                toastr.error("No data found!!")
                            }
                        } else {
                            toastr.error(response.data.Message);
                        }
                        $scope.dataLoading = false;
                    }, function (err) {
                        toastr.error("Some error occured while fetching data");
                        $scope.dataLoading = false;
                    })
            } else {
                toastr.error("Fill all the required fields !!", "Validation Error");
            }
        }

        $scope.getAcademicSessionList = function () {
            $scope.dataLoading = true;
            adminService.getAcademicSessionList(ConstantData.SessionStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.AcademicSessionList = response.data.AcademicSessionList;
                        //$scope.AcademicSessionId = $scope.AcademicSessionList[1].AcademicSessionId;
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
            $scope.ClassNo = null;
            $scope.SectionId = null;
            $scope.MarksheetList = [];
            $scope.dataLoading = true;
            //adminService.getClassListDistinct(ConstantData.ClassStatus[1].Key)
            adminService.getClassListDistinctForClassTeacher(1, $scope.AcademicSessionId)
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
        //$scope.getClassList();

        $scope.changeClass = function () {
            $scope.MarksheetList = [];
            $scope.SectionList = [];
            angular.forEach($scope.MainSectionList, function (section, key) {
                if (section.ClassNo == $scope.ClassNo) {
                    $scope.SectionList.push(section);
                }
            })
        }

        $scope.changeSelectAllHY = function () {
            angular.forEach($scope.MarksheetList, function (marksheet, key) {
                if ($scope.selectAllHY) 
                    marksheet.AllowHYMarksheetDownload = true;
                else
                    marksheet.AllowHYMarksheetDownload = false;
            })
        }

        $scope.changeSelectAllAN = function () {
            angular.forEach($scope.MarksheetList, function (marksheet, key) {
                if ($scope.selectAllAN) 
                    marksheet.AllowANMarksheetDownload = true;
                else
                    marksheet.AllowANMarksheetDownload = false;
            })
        }

        $scope.getSectionList = function () {
            $scope.dataLoading = true;
            adminService.getSectionListForClassTeacher(1, 0, $scope.AcademicSessionId)
            //adminService.getSectionList(ConstantData.SectionStatus[1].Key)
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
        //$scope.getSectionList();

        $scope.saveAllowMarksheet = function () {
            $scope.IsSubmitted = true;
            if ($scope.isFormMarksheetValid) {
                $scope.dataLoading = true;
                adminService.saveAllowMarksheet($scope.MarksheetList)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            $scope.MarksheetList = [];
                            toastr.success("Data Saved");
                        } else {
                            toastr.error(response.data.Message);
                        }
                        $scope.dataLoading = false;
                    }, function (err) {
                        toastr.error("Some error occured while fetching data");
                        $scope.dataLoading = false;
                    })
            } else {
                toastr.error("Fill all the required fields !!", "Validation Error");
            }
        }
    }
})();
