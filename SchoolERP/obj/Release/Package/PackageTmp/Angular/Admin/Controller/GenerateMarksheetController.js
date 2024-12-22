(function () {
    'use strict';

    angular
        .module('app')
        .controller('GenerateMarksheetController', GenerateMarksheetController);

    GenerateMarksheetController.$inject = ['$scope', 'adminService', 'Factory', 'ConstantData', '$location'];

    function GenerateMarksheetController($scope, adminService, Factory, ConstantData, $location) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.ExamTermList = ConstantData.ExamTerm;
        $scope.ExamTerm = null;

        $scope.$watch('formMarksheet.$valid', function (value) {
            $scope.isFormMarksheetValid = value;
        })

        $scope.generateMarksheet = function () {
            $scope.IsSubmitted = true;
            if ($scope.isFormMarksheetValid) {
                $scope.dataLoading = true;
                adminService.generateMarksheet($scope.AcademicSessionId, $scope.SectionId, $scope.ExamTerm)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            toastr.success("Marksheet generated successfully.", "Success");
                            $scope.IsSubmitted = false;
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
                        $scope.AcademicSessionId = $scope.AcademicSessionList[$scope.AcademicSessionList.length - 2].AcademicSessionId;
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
            adminService.getClassListDistinct(ConstantData.ClassStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ClassList = response.data.ClassList;
                        var index = { ClassNo: 0, ClassName: '--Select Class--' };
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
                if (section.ClassNo == $scope.ClassNo) {
                    $scope.SectionList.push(section);
                }
            })
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
    }
})();
