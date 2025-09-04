using AiryPay.Domain.Entities.ShopComplaints;
using Discord;
using Discord.Interactions;

namespace AiryPay.Discord.Modals;

public class ComplaintModal : IModal
{
    public string Title { get; }  = "\ud83d\udea8 Shop complaint";
    
    public ComplaintModal() { }
    public ComplaintModal(string title, string reason, string details)
    {
        Title = title;
        Reason = reason;
        Details = details;
    }
    
    [InputLabel("Reason")]
    [RequiredInput]
    [ModalTextInput(
        "ComplaintModal.Reason",
        TextInputStyle.Short,
        "Choose a reason for complaint",
        5, ShopComplaint.Constants.ReasonMaxLength)]
    public string Reason { get; set; } = string.Empty;
    
    [InputLabel("Details")]
    [RequiredInput]
    [ModalTextInput(
        "ComplaintModal.Details",
        TextInputStyle.Paragraph,
        "Please provide any details",
        0, ShopComplaint.Constants.DetailsMaxLength)]
    public string Details { get; set; } = string.Empty;
}