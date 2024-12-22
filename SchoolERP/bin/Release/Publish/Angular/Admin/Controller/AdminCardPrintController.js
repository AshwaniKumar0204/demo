(function () {
    'use strict';

    angular
        .module('app')
        .controller('AdmitCardPrintController', AdmitCardPrintController);

    AdmitCardPrintController.$inject = ['$scope', 'adminService', 'Factory', 'ConstantData', '$location'];

    function AdmitCardPrintController($scope, adminService, Factory, ConstantData, $location) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.ExamTermList = [
            { Key: 1, Value: 'Term-1 (HY)' },
            { Key: 2, Value: 'Term-2 (AN)' },
            { Key: 3, Value: 'Final' }
        ];

        $scope.$watch('formValidation.$valid', function (value) {
            $scope.isFormValid = value;
        })

        $scope.getAcademicSessionList = function () {
            $scope.dataLoading = true;
            adminService.getAcademicSessionList(ConstantData.SessionStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
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
            $scope.ClassNo = null;
            $scope.SectionId = null;
            $scope.dataLoading = true;
            //adminService.getClassListDistinct(1)
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
            $scope.SectionList = [];
            angular.forEach($scope.MainSectionList, function (section, key) {
                if (section.ClassNo == $scope.ClassNo) {
                    $scope.SectionList.push(section);
                }
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

        $scope.getExaminationList = function () {
            $scope.dataLoading = true;
            adminService.getExaminationList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ExaminationList = response.data.ExaminationList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getExaminationList();

        $scope.printReport = function (ActionName, docType) {
            $scope.IsSubmitted = true;
            if ($scope.isFormValid) {
                var ExaminationId = 0;
                if ($scope.ExaminationId != null) {
                    ExaminationId = $scope.ExaminationId;
                }
                var se = 0, cl = 0;
                if ($scope.SectionId)
                    se = $scope.SectionId;
                if ($scope.ClassNo)
                    cl = $scope.ClassNo;

                window.open("/print/" + ActionName + "?ac=" + $scope.AcademicSessionId + "&se=" +se+ "&cl=" +cl+ "&em=" + ExaminationId);
            } else {
                toastr.error("Fill all required fields !!");
            }
        }
    }
})();
