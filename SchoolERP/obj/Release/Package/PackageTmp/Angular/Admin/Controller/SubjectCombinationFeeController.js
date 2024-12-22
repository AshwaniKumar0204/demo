(function () {
    'use strict';

    angular
        .module('app')
        .controller('SubjectCombinationFeeController', SubjectCombinationFeeController);

    SubjectCombinationFeeController.$inject = ["$scope", "adminService"];

    function SubjectCombinationFeeController($scope, adminService) {
        $scope.checkSysytemLogin();
        $scope.SubjectCombinationFeeList = [];
        $scope.AcademicSessionId = null;
        $scope.ClassId = null;
        $scope.StudentTypeId = null;

        $scope.getClassList = function () {
            $scope.dataLoading = true;
            adminService.getClassList(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ClassList = response.data.ClassList;
                        var index = { ClassId: null, ClassName: '--Select Course--' };
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

        $scope.getSubjectCombinationFeeList = function () {
            if ($scope.AcademicSessionId > 0 && $scope.ClassId > 0) {
                $scope.dataLoading = true;
                adminService.getSubjectCombinationFeeList($scope.ClassId, $scope.AcademicSessionId)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            $scope.SubjectCombinationFeeList = response.data.SubjectCombinationFeeList;
                            if ($scope.SubjectCombinationFeeList.length == 0) {
                                toastr.error("No data found.");
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
                $scope.SubjectCombinationFeeList = [];
            }
        }

        $scope.saveSubjectCombinationFee = function () {
            $scope.dataLoading = true;
            adminService.saveSubjectCombinationFee($scope.SubjectCombinationFeeList)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.getSubjectCombinationFeeList();
                        toastr.success("Subject Combination fee updated successfully.")
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
