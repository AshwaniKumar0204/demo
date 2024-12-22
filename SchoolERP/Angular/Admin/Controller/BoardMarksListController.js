(function () {
    'use strict';

    angular
        .module('app')
        .controller('BoardMarksListController', BoardMarksListController);

    BoardMarksListController.$inject = ['$scope', 'adminService', 'Factory', 'ConstantData', 'loadDataService'];

    function BoardMarksListController($scope, adminService, Factory, ConstantData, loadDataService) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[0];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };


        $scope.getClassStreamTypeList = function () {
            $scope.SubjectOptionOne = null;
            $scope.SubjectOptionTwo = null;
            $scope.ClassStreamTypeId = null;
            $scope.dataLoading = true;
            adminService.getClassStreamTypeList(1, $scope.ClassNo)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ClassStreamTypeList = response.data.ClassStreamTypeList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.ClassNo = 11;
        $scope.getClassStreamTypeList();

        $scope.getSubjectOptionList = function () {
            if ($scope.ClassStreamTypeId == null || $scope.ClassStreamTypeId == undefined) {
                return;
            }

            $scope.dataLoading = true;
            adminService.getSubjectOptionList($scope.ClassNo, $scope.ClassStreamTypeId, 0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.SubjectOptionOneList = response.data.SubjectOptionList;
                        //$scope.changeSubjectOptionOne();
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
            //$scope.changeSubjectOptionOne();
        }

        $scope.getAcademicSessionList = function () {
            $scope.dataLoading = true;
            adminService.getAcademicSessionList(ConstantData.SessionStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.AcademicSessionList = response.data.AcademicSessionList;
                        $scope.AcademicSessionId = response.data.AcademicSessionList[0].AcademicSessionId;
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

        $scope.getBoardMarksStatusList = function () {
            $scope.dataLoading = true;
            adminService.getBoardMarksStatusList(ConstantData.SessionStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.BoardMarksStatusList = response.data.BoardMarksStatusList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getBoardMarksStatusList();

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

        $scope.getRegistrationFormList = function () {
            $scope.dataLoading = true;
            adminService.getSearchRegistrationFormList(0, $scope.AcademicSessionId, $scope.ClassNo)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.RegistrationFormList = response.data.RegistrationFormList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getRegistrationFormList();

        $scope.changeRegistrationForm = function () {
            $scope.SelectedRegistrationForm = {};
            for (var i = 0; i < $scope.RegistrationFormList.length; i++) {
                if ($scope.RegistrationFormId == $scope.RegistrationFormList[i].RegistrationFormId) {
                    $scope.SelectedRegistrationForm = $scope.RegistrationFormList[i];
                    break;
                }
            }
        }

        $scope.$watch('formUnit.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.getRegistrationOnlineList = function () {
            $scope.IsSubmitted = true;
            if (!$scope.IsFormValid) {
                toastr.error("Fill all required fields!");
                return;
            }
            if (!$scope.IsSameSchool)
                $scope.ResultType = 1;
            var obj = {
                AcademicSessionId: $scope.AcademicSessionId,
                ClassNo: $scope.ClassNo,
                RegistrationFormId: $scope.RegistrationFormId,
                ClassStreamTypeId: $scope.ClassStreamTypeId,
                SubjectOptionOne: $scope.SubjectOptionOne,
                SubjectOptionTwo: $scope.SubjectOptionTwo,
                IsSameSchool: $scope.IsSameSchool,
                IsSeen: $scope.IsSeen,
                AdmissionType: $scope.AdmissionType,
                BoardMarksStatus: $scope.BoardMarksStatus,
            }
            $scope.dataLoading = true;
            adminService.getBoardMarksList(obj)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.RegistrationOnlineList = response.data.RegistrationOnlineList;
                        angular.forEach($scope.RegistrationOnlineList, function (r1) {
                            r1.MathSciencePercentage = (r1.Maths10Total + r1.Science10Total) / 2;
                        })
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.viewFee = function (r1) {
            $scope.SelectedRegistrationOnline = r1;
            $('#modal_popUp').modal('show');
        }
        $scope.CheckedAll = function () {
            var IsSeenAll = document.getElementById('IsSeenAll').checked;
            angular.forEach($scope.RegistrationOnlineList, function (registrationOnline, key1) {
                if (IsSeenAll)
                    registrationOnline.IsSeen = true;
                else
                    registrationOnline.IsSeen = false;
            })
        }
        $scope.changeChecked = function (registrationOnline) {
            registrationOnline.IsSeen = !registrationOnline.IsSeen;
        }

        $scope.updateBoardMarkSeen = function () {
            var RegistrationList = [];
            angular.forEach($scope.RegistrationOnlineList, function (registration, key1) {
                RegistrationList.push({ BoardMarksId: registration.BoardMarksId, IsSeen: registration.IsSeen, BoardMarksStatus: registration.BoardMarksStatus });
            })

            $scope.dataLoading = true;
            adminService.updateBoardMarksSeen(RegistrationList)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("Data Updated Successfully.");
                        $scope.RegistrationOnlineList = [];
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
