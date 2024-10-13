document.addEventListener("DOMContentLoaded", function () {
    // Stepper element
    var element = document.querySelector("#kt_modal_create_app_stepper");

    // Initialize Stepper
    var stepper = new KTStepper(element);

    // Handle next step
    stepper.on("kt.stepper.next", function (stepper) {
        if (validateCurrentStep(stepper)) {
            stepper.goNext(); // Go to the next step
        } else {
            showError("Please fill in all required fields."); // Improved notification method
        }
    });

    // Handle previous step
    stepper.on("kt.stepper.previous", function (stepper) {
        stepper.goPrevious(); // Go to the previous step
    });

    // Validate the current step's inputs
    function validateCurrentStep(stepper) {
        var currentStep = stepper.getCurrentStepIndex();
        //var requiredInputs = currentStep.querySelectorAll("input[required], select[required]"); // Get required inputs

        let allValid = true; // Track overall validation status

        //requiredInputs.forEach(input => {
        //    if (!input.value.trim()) { // Check if the input is empty or whitespace
        //        input.classList.add("is-invalid"); // Add invalid class for styling
        //        allValid = false; // Mark as invalid
        //    } else {
        //        input.classList.remove("is-invalid"); // Remove invalid class if valid
        //    }
        //});

        return allValid; // Return overall validation status
    }

    // Show error notification
    function showError(message) {
        // Use a modal, toast, or any preferred notification method for user feedback
        alert(message); // Simple alert for now; replace with your own logic
    }

    // Optional: Handle step completion
    stepper.on("kt.stepper.completed", function () {
        // Code to execute when all steps are completed
        alert("All steps are completed!"); // Replace with your own logic
    });
});
