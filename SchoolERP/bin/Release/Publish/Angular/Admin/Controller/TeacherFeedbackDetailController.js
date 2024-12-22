(function () {
    'use strict';

    angular
        .module('app')
        .controller('TeacherFeedbackDetailController', TeacherFeedbackDetailController);

    TeacherFeedbackDetailController.$inject = ["$scope", "adminService", "loadDataService", "$routeParams", "$location"];
    function TeacherFeedbackDetailController($scope, adminService, loadDataService, $routeParams, $location) {
        $scope.checkSysytemLogin();

        $scope.getTeacherPerformanceParameterList = function (PerformanceId) {
            $scope.dataLoading = true;
            adminService.getTeacherPerformanceParameterList(PerformanceId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.PerformanceParameterList = response.data.PerformanceParameterList;
                        $scope.TeacherExperienceList = response.data.TeacherExperienceList;
                        $scope.TeacherCoScholasticList = response.data.TeacherCoScholasticList;
                        $scope.TrainingProgrammeList = response.data.TrainingProgrammeList;
                        $scope.TeachingLearningAidList = response.data.TeachingLearningAidList;
                        $scope.TeacherBookReadList = response.data.TeacherBookReadList;
                        $scope.BookReading = response.data.BookReading;
                        $scope.ClassTaught = response.data.ClassTaught;
                        $scope.CoScholastic = response.data.CoScholastic;
                        $scope.TeacherLearningAidsPramaterId = response.data.TeacherLearningAids;
                        $scope.TeachingExprienceId = response.data.TeachingExprienceId;
                        $scope.TrainingProgrammePrameterId = response.data.TrainingProgrammePrameterId;
                        $scope.AcademicSession = response.data.AcademicSession;
                        $scope.AcademicSessionId = response.data.AcademicSession.AcademicSessionId;
                        $scope.TeacherId = response.data.AcademicSession.TeacherId;
                        $scope.getTeacherSubjectListForFeedback();
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.getTeacherParameterList = function () {
            $scope.dataLoading = true;
            adminService.getTeacherParameterList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ParameterList = response.data.ParameterList;
                        $scope.AcademicSession = response.data.AcademicSession;
                        $scope.AcademicSessionId = response.data.AcademicSession.AcademicSessionId;
                        $scope.BookReading = response.data.BookReading;
                        $scope.ClassTaught = response.data.ClassTaught;
                        $scope.CoScholastic = response.data.CoScholastic;
                        $scope.TeacherLearningAidsPramaterId = response.data.TeacherLearningAids;
                        $scope.TeachingExprienceId = response.data.TeachingExprienceId;
                        $scope.TrainingProgrammePrameterId = response.data.TrainingProgramme;
                        $scope.getTeacherSubjectListForFeedback();
                        $scope.getTeachingLearningAidDetail();
                        $scope.isAlreadySubmitted = true;
                    } else if (response.data.Message == 'submitted') {
                        $scope.isAlreadySubmitted = true;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.getTeacherFeedbackListFromPricipal = function () {
            $scope.dataLoading = true;
            adminService.getTeacherFeedbackListFromPricipal($scope.PerformanceId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.PerformanceParameterListFromPricipal = response.data.PerformanceParameterList;
                        //console.log(response.data);
                        //angular.forEach($scope.PerformanceParameterListFromPricipal, function (x1) {
                        //    angular.forEach(x1.ParameterList, function (x2) {
                        //        if (x2.RatingDetailList.length > 0 && x2.RatingDetailId == 0)
                        //            x2.RatingDetailId = null;
                        //    });
                        //});
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.getTeacherSubjectListForFeedback = function () {
            $scope.dataLoading = true;
            adminService.getTeacherSubjectListForFeedback($scope.AcademicSessionId, $scope.TeacherId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.TeacherSubjectList = response.data.TeacherSubjectList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.saveTeacherPerformanceByPrincipal = function () {
            var PerformanceParameterList = [];
            for (var i = 0; i < $scope.PerformanceParameterListFromPricipal.length; i++) {
                for (var j = 0; j < $scope.PerformanceParameterListFromPricipal[i].ParameterList.length; j++) {
                    var x2 = $scope.PerformanceParameterListFromPricipal[i].ParameterList[j];
                    if (x2.RatingDetailId)
                        PerformanceParameterList.push({
                            PerformanceParameterId: x2.PerformanceParameterId,
                            PerformanceId: x2.PerformanceId,
                            Remarks: x2.Remarks,
                            ParameterId: x2.ParameterId,
                            RatingDetailId: x2.RatingDetailId
                        });
                    else {
                        toastr.error("Rating of " + x2.ParameterName + " is required.");
                        return;
                    }
                }
            }

            $scope.dataLoading = true;
            adminService.saveTeacherPerformanceByPrincipal(PerformanceParameterList)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("Data Updated Successfully.");
                        $scope.PerformanceParameterListFromPricipal = [];
                        $scope.PerformanceParameterList = [];
                        $scope.PerformanceId = null;
                        if ($routeParams.id) {
                            $location.path('TeacherFeedbackList')
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

        if ($routeParams.id) {
            $scope.PerformanceId = $routeParams.id;
            $scope.getTeacherPerformanceParameterList($routeParams.id);
            $scope.getTeacherFeedbackListFromPricipal();
        }
    }
})();
