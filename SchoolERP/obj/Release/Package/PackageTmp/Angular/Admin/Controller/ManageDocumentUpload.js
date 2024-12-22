(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageDocumentUploadController', ManageDocumentUploadController);

    ManageDocumentUploadController.$inject = ["$scope", "adminService", "ConstantData", "Factory"];

    function ManageDocumentUploadController($scope, adminService, ConstantData, Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.Doc = {};
        $scope.CurrentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = Factory.SizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
      
        $scope.DocumentStatuslist = ConstantData.DocumentStatus;
        $scope.abc = "Adarsh verma"
    
     

        //////////////////////////////////////Addhar Card///////////////////////////////////////////////
        $scope.AddharCard = function (element) {
            if (element.files[0].size < 512000) {
                $scope.UploadFileFormat = element.files[0].name.split('.').pop();
                var reader = new FileReader();
                reader.onload = function (event) {
                    var dataUrl = event.target.result;
                    var base64Data = dataUrl.substr(dataUrl.indexOf('base64,') + 'base64,'.length);
                    $scope.AddharCard = base64Data;
                    $scope.$apply()
                    $scope.IsImage2Selected = true;
                }
                reader.readAsDataURL(element.files[0]);
            } else {
                document.getElementById("AddharCard").value = "";
                toastr.success("File size should be less than 500 KB.");
            }
        }




        //////////////////////////////////////Addhar Card///////////////////////////////////////////////
        $scope.MatrixCertifiacate = function (element) {
            if (element.files[0].size < 512000) {
                $scope.UploadFileFormat = element.files[0].name.split('.').pop();
                var reader = new FileReader();
                reader.onload = function (event) {
                    var dataUrl = event.target.result;
                    var base64Data = dataUrl.substr(dataUrl.indexOf('base64,') + 'base64,'.length);
                    $scope.MatrixCertifiacate = base64Data;
                    $scope.$apply()
                    $scope.IsImage2Selected = true;
                }
                reader.readAsDataURL(element.files[0]);
            } else {
                document.getElementById("MatrixCertifiacate").value = "";
                toastr.success("File size should be less than 500 KB.");
            }
        }

        //////////////////////////////////////Addhar Card///////////////////////////////////////////////
        $scope.IntermediateCertificate = function (element) {
            if (element.files[0].size < 512000) {
                $scope.UploadFileFormat = element.files[0].name.split('.').pop();
                var reader = new FileReader();
                reader.onload = function (event) {
                    var dataUrl = event.target.result;
                    var base64Data = dataUrl.substr(dataUrl.indexOf('base64,') + 'base64,'.length);
                    $scope.IntermediateCertificate = base64Data;
                    $scope.$apply()
                    $scope.IsImage2Selected = true;
                }
                reader.readAsDataURL(element.files[0]);
            } else {
                document.getElementById("IntermediateCertificate").value = "";
                toastr.success("File size should be less than 500 KB.");
            }
        }



        //////////////////////////////////////Ten Passing Certificate ///////////////////////////////////////////////
        $scope.TenPassingCertificate = function (element) {
            if (element.files[0].size < 512000) {
                $scope.UploadFileFormat = element.files[0].name.split('.').pop();
                var reader = new FileReader();
                reader.onload = function (event) {
                    var dataUrl = event.target.result;
                    var base64Data = dataUrl.substr(dataUrl.indexOf('base64,') + 'base64,'.length);
                    $scope.TenPassingCertificate = base64Data;
                    $scope.$apply()
                    $scope.IsImage2Selected = true;
                }
                reader.readAsDataURL(element.files[0]);
            } else {
                document.getElementById("TenPassingCertificate").value = "";
                toastr.success("File size should be less than 500 KB.");
            }
        }



        //////////////////////////////////////Twelve Passing Certificate ///////////////////////////////////////////////
        $scope.TwelvePassingCertificate = function (element) {
            if (element.files[0].size < 512000) {
                $scope.UploadFileFormat = element.files[0].name.split('.').pop();
                var reader = new FileReader();
                reader.onload = function (event) {
                    var dataUrl = event.target.result;
                    var base64Data = dataUrl.substr(dataUrl.indexOf('base64,') + 'base64,'.length);
                    $scope.TwelvePassingCertificate = base64Data;
                    $scope.$apply()
                    $scope.IsImage2Selected = true;
                }
                reader.readAsDataURL(element.files[0]);
            } else {
                document.getElementById("TwelvePassingCertificate").value = "";
                toastr.success("File size should be less than 500 KB.");
            }
        }


        //////////////////////////////////////CLC ///////////////////////////////////////////////
        $scope.CLC = function (element) {
            if (element.files[0].size < 512000) {
                $scope.UploadFileFormat = element.files[0].name.split('.').pop();
                var reader = new FileReader();
                reader.onload = function (event) {
                    var dataUrl = event.target.result;
                    var base64Data = dataUrl.substr(dataUrl.indexOf('base64,') + 'base64,'.length);
                    $scope.CLC = base64Data;
                    $scope.$apply()
                    $scope.IsImage2Selected = true;
                }
                reader.readAsDataURL(element.files[0]);
            } else {
                document.getElementById("CLC").value = "";
                toastr.success("File size should be less than 500 KB.");
            }
        }

        //////////////////////////////////////MigrationCertificate ///////////////////////////////////////////////
        $scope.MigrationCertificate = function (element) {
            if (element.files[0].size < 512000) {
                $scope.UploadFileFormat = element.files[0].name.split('.').pop();
                var reader = new FileReader();
                reader.onload = function (event) {
                    var dataUrl = event.target.result;
                    var base64Data = dataUrl.substr(dataUrl.indexOf('base64,') + 'base64,'.length);
                    $scope.MigrationCertificate = base64Data;
                    $scope.$apply()
                    $scope.IsImage2Selected = true;
                }
                reader.readAsDataURL(element.files[0]);
            } else {
                document.getElementById("MigrationCertificate").value = "";
                toastr.success("File size should be less than 500 KB.");
            }
        }


        $scope.Studentlist = function () {
            adminService.Studentlist()
                .then(function (response) {
                    if (response.data.Message == 'Success') {

                        $scope.Studentlist = response.data.documetlist;
                    }
                    else {
                        toastr.error(response.data.Message);
                    }
                });
        }
        $scope.Studentlist();



        $scope.afterStudentSelected = (obj) => {

            if (obj != null && obj != undefined) {
                $scope.AdmissionNo = obj.originalObject.AdmissionNo;
                $scope.Doc.StudentId = obj.originalObject.StudentId;

            }
            else {
                $scope.AdmissionNo = undefined;
            }
        };

        $scope.editStudentDocument = function (doc) {
         
            $scope.Doc = doc;
            $scope.$broadcast('angucomplete-alt:changeInput', 'angu_125', doc.AdmissionNo);

            $('#modal_Student_Charge').modal('show');
        }


        $scope.deleteConfirmation = function (DocumentID) {
            $scope.DocumentID = DocumentID;
            $('#modal_confirmation').modal('show');
        }
        $scope.deleteDocument = function () {
            $scope.dataLoading = true;
            adminService.deleteDocument($scope.DocumentID)
                .then(function (response) {
                    if (response.data.message == 'Success') {
                        toastr.success("Student Document deleted successfully.", "Deleted Successfully.")
                       
                        $('#modal_confirmation').modal('hide');
                    } else {
                        toastr.error(response.data.message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

       

        $scope.newDocument = function () {
            $scope.formDocumentUpload.$setPristine();
            $scope.formDocumentUpload.$setUntouched();
            $('#modal_Student_Charge').modal('show');
        }
        $scope.SaveDocument = function () {
            if ($scope.Doc) {
                var Documentad = {
                    DocumentID: $scope.Doc.DocumentID,
                    StudentId: $scope.Doc.StudentId,
                    UploadFileFormat: $scope.UploadFileFormat,
                    MatrixCertifiacate: $scope.MatrixCertifiacate,
                    DocumentStatus: $scope.Doc.DocumentStatus,
                    IntermediateCertificate: $scope.IntermediateCertificate,
                    AddharCard: $scope.AddharCard,
                    TenPassingCertificate: $scope.TenPassingCertificate,
                    TwelvePassingCertificate: $scope.TwelvePassingCertificate,
                    CLC: $scope.CLC,
                    MigrationCertificate: $scope.MigrationCertificate,
                };

                // Call adminService to save the document
                adminService.saveDocument(Documentad)
                    .then(function (response) {
                        if (response.data.Message === 'Success') {
                            if ($scope.Doc.DocumentID > 0) {
                                toastr.success("Student Document Updated Successfully.");
                            } else {
                                toastr.success("Student Document Created Successfully.");
                            }
                        } else {
                            toastr.error(response.data.Message);
                        }
                    });
            } else {
                toastr.error("Fill all fields!", "Validation Error");
            }
   
        };






        $scope.DocumentList = function () {
            adminService.DocumentList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {

                        $scope.DocumentList = response.data.Documentlist;
                    }
                    else {
                        toastr.error(response.data.Message);
                    }
                });
        }
        $scope.DocumentList();
     

    }

})();

