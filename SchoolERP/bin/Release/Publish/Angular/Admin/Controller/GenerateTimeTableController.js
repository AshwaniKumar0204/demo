(function () {
    'use strict';

    angular
        .module('app')
        .controller('GenerateTimeTableController', GenerateTimeTableController);

    GenerateTimeTableController.$inject = ['$scope', 'adminService', 'ConstantData'];

    function GenerateTimeTableController($scope, adminService, ConstantData) {
        $scope.checkSysytemLogin();
        $scope.$watch('myForm.$valid', function (value) {
            $scope.isFormValid = value;
        })

        $scope.getClassList = function () {
            $scope.dataLoading = true;
            adminService.getClassListDistinct(1)
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
                if (section.ClassNo == $scope.ClassNo) {
                    $scope.SectionList.push(section);
                }
            })
        }

        $scope.changeSession = function () {
            for (var i = 0; i < $scope.AcademicSessionList.length; i++) {
                var academicSession = $scope.AcademicSessionList[i];
                if (academicSession.AcademicSessionId == $scope.AcademicSessionId) {
                    $scope.SelectedSession = academicSession;
                    break;
                }
            }
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

        $scope.getAcademicSessionList = function () {
            $scope.dataLoading = true;
            adminService.getAcademicSessionList(ConstantData.SessionStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.AcademicSessionList = response.data.AcademicSessionList;
                        $scope.changeSession();
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

        $scope.getClassTimeTableList = function () {
            if ($scope.AcademicSessionId == null) {
                toastr.error("Invalid Session !!");
                return;
            }
            var SectionId = 0;
            if ($scope.SectionId > 0)
                SectionId = $scope.SectionId;
            var ClassNo = 0;
            if ($scope.ClassNo > 0)
                ClassNo = $scope.ClassNo;
            $scope.dataLoading = true;
            adminService.getClassTimeTableList(ClassNo, SectionId, $scope.AcademicSessionId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.TimeTablePeriodList = response.data.TimeTablePeriodList;
                        $scope.ClassTimeTableList = response.data.ClassTimeTableList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.generateTimeTable = function () {
            $scope.IsSubmitted = true;
            if (!$scope.isFormValid) {
                toastr.error("Please fill the all required fields!");
                return;
            }
            var SectionId = 0;
            if ($scope.SectionId > 0)
                SectionId = $scope.SectionId;
            $scope.dataLoading = true;
            adminService.generateTimeTable($scope.ClassNo, SectionId, $scope.AcademicSessionId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.getClassTimeTableList();
                        toastr.success("Time Table Generated Successfully.");
                    } else {
                        toastr.error(response.data.Message);
                        $scope.dataLoading = false;
                    }
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.deleteConfirmation = function (ClassTimeTableId) {
            $scope.ClassTimeTableId = ClassTimeTableId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteTimeTable = function () {
            if ($scope.ClassTimeTableId > 0) {
                $scope.deleteClassTimeTable();
            } else {
                var SectionId = 0;
                if ($scope.SectionId > 0)
                    SectionId = $scope.SectionId;
                var ClassNo = 0;
                if ($scope.ClassNo > 0)
                    ClassNo = $scope.ClassNo;
                $scope.dataLoading = true;
                adminService.deleteTimeTable(ClassNo, SectionId, $scope.AcademicSessionId)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            toastr.success("Deleted Successfully.")
                            $('#modal_confirmation').modal('hide');
                            $scope.getClassTimeTableList();
                        } else {
                            toastr.error(response.data.Message);
                            $scope.dataLoading = false;
                        }
                    }, function (err) {
                        toastr.error("Some error occured while fetching data");
                        $scope.dataLoading = false;
                    })
            }
        }

        $scope.deleteClassTimeTable = function () {
            $scope.dataLoading = true;
            adminService.deleteClassTimeTable($scope.ClassTimeTableId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("Deleted Successfully.")
                        $('#modal_confirmation').modal('hide');
                        $scope.getClassTimeTableList();
                        $scope.ClassTimeTableId = null;
                    } else {
                        toastr.error(response.data.Message);
                        $scope.dataLoading = false;
                    }
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.editClassTimeTable = function (ClassTimeTableId) {

        }
    }
})();
