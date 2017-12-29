using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcpe.ENewsletters.Templates
{
    using Gcpe.ENewsletters.Data.Entity;

    public class Email
    {


        public static string HtmlWrapper(string subject, string body, string baseUrlForPublicSite, bool isTest)
        {
            //+ "<link rel=\"stylesheet\" href=\"" + Utility._Base_URL_Public + "css/EmailStyles.css\" type=\"text/css\" /> \n";

            //http://htmlemailboilerplate.com/#f1

            //{0} = Title or Subject
            //{1} = Body of Newsletter
            //{2} = Tracker Image for Reporting

            string wrapper_V2 = 
                "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\"> " +
                "<html xmlns=\"http://www.w3.org/1999/xhtml\">" + 
                "<head>" + 
                "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />" + 
                "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"/>" + 
                string.Format("<title>{0}</title>", subject) + 
                "<style type=\"text/css\">" +


                //026./***********
                //027.Originally based on The MailChimp Reset from Fabio Carneiro, MailChimp User Experience Design
                //028.More info and templates on Github: https://github.com/mailchimp/Email-Blueprints
                //029.http://www.mailchimp.com & http://www.fabio-carneiro.com
                //030. 
                //031.INLINE: Yes.
                //032.***********/ 
                //033./* Client-specific Styles */
                "#outlook a {padding:0;} " + /* Force Outlook to provide a "view in browser" menu link. */
                "body{width:100% !important; -webkit-text-size-adjust:100%; -ms-text-size-adjust:100%; margin:0; padding:0;}" +
        
                //036./* Prevent Webkit and Windows Mobile platforms from changing default font sizes, while not breaking desktop design. */
                ".ExternalClass {width:100%;} " + /* Force Hotmail to display emails at full width */ 
                ".ExternalClass, .ExternalClass p, .ExternalClass span, .ExternalClass font, .ExternalClass td, .ExternalClass div {line-height: 100%;} " + /* Force Hotmail to display normal line spacing.  More on that: http://www.emailonacid.com/forum/viewthread/43/ */
                "#backgroundTable {margin:0; padding:0; width:100% !important; line-height: 100% !important;} " +
                //040./* End reset */


                //042./* Some sensible defaults for images 
                //043.1. "-ms-interpolation-mode: bicubic" works to help ie properly resize images in IE. (if you are resizing them using the width and height attributes)
                //044.2. "border:none" removes border when linking images.
                //045.3. Updated the common Gmail/Hotmail image display fix: Gmail and Hotmail unwantedly adds in an extra space below images when using non IE browsers. You may not always want all of your images to be block elements. Apply the "image_fix" class to any image you need to fix.
                //046. 
                //047.Bring inline: Yes.
                //048.*/
                "img {outline:none; text-decoration:none; -ms-interpolation-mode: bicubic;}" +
                "a img {border:none;} " +
                ".image_fix {display:block;} " +


        //053./** Yahoo paragraph fix: removes the proper spacing or the paragraph (p) tag. To correct we set the top/bottom margin to 1em in the head of the document. Simple fix with little effect on other styling. NOTE: It is also common to use two breaks instead of the paragraph tag but I think this way is cleaner and more semantic. NOTE: This example recommends 1em. More info on setting web defaults: http://www.w3.org/TR/CSS21/sample.html or http://meiert.com/en/blog/20070922/user-agent-style-sheets/
        //054. 
        //055.Bring inline: Yes.
        //056.**/
        //057.p {margin: 1em 0;}
        //058. 
        //059./** Hotmail header color reset: Hotmail replaces your header color styles with a green color on H2, H3, H4, H5, and H6 tags. In this example, the color is reset to black for a non-linked header, blue for a linked header, red for an active header (limited support), and purple for a visited header (limited support).  Replace with your choice of color. The !important is really what is overriding Hotmail's styling. Hotmail also sets the H1 and H2 tags to the same size.
        //060. 
        //061.Bring inline: Yes.
        //062.**/
        //063.h1, h2, h3, h4, h5, h6 {color: black !important;}
        //064. 
        //065.h1 a, h2 a, h3 a, h4 a, h5 a, h6 a {color: blue !important;}
        //066. 
        //067.h1 a:active, h2 a:active,  h3 a:active, h4 a:active, h5 a:active, h6 a:active {
        //068.color: red !important; /* Preferably not the same color as the normal header link color.  There is limited support for psuedo classes in email clients, this was added just for good measure. */
        //069.}
        //070. 
        //071.h1 a:visited, h2 a:visited,  h3 a:visited, h4 a:visited, h5 a:visited, h6 a:visited {
        //072.color: purple !important; /* Preferably not the same color as the normal header link color. There is limited support for psuedo classes in email clients, this was added just for good measure. */
        //073.}
        //074. 
        
                //075./** Outlook 07, 10 Padding issue: These "newer" versions of Outlook add some padding around table cells potentially throwing off your perfectly pixeled table.  The issue can cause added space and also throw off borders completely.  Use this fix in your header or inline to safely fix your table woes.
                //076. 
                //077.More info: http://www.ianhoar.com/2008/04/29/outlook-2007-borders-and-1px-padding-on-table-cells/
                //078.http://www.campaignmonitor.com/blog/post/3392/1px-borders-padding-on-table-cells-in-outlook-07/
                //079. 
                //080.H/T @edmelly
                //081. 
                //082.Bring inline: No.
                //083.**/
                "table td {border-collapse: collapse;}" +
 
        //086./* Styling your links has become much simpler with the new Yahoo.  In fact, it falls in line with the main credo of styling in email, bring your styles inline.  Your link colors will be uniform across clients when brought inline.
        //087. 
        //088.Bring inline: Yes. */
        //089.a {color: orange;}
        //090. 
        //091./* Or to go the gold star route...
        //092.a:link { color: orange; }
        //093.a:visited { color: blue; }
        //094.a:hover { color: green; }
        //095.*/
        //096. 

                //097./***************************************************
                //098.****************************************************
                //099.MOBILE TARGETING
                //100. 
                //101.Use @media queries with care.  You should not bring these styles inline -- so it's recommended to apply them AFTER you bring the other stlying inline.
                //102. 
                //103.Note: test carefully with Yahoo.
                //104.Note 2: Don't bring anything below this line inline.
                //105.****************************************************
                //106.***************************************************/
        //107. 
        //108./* NOTE: To properly use @media queries and play nice with yahoo mail, use attribute selectors in place of class, id declarations.
        //109.table[class=classname]
        //110.Read more: http://www.campaignmonitor.com/blog/post/3457/media-query-issues-in-yahoo-mail-mobile-email/
        //111.*/
        //112.@media only screen and (max-device-width: 480px) {
        //113. 
        //114./* A nice and clean way to target phone numbers you want clickable and avoid a mobile phone from linking other numbers that look like, but are not phone numbers.  Use these two blocks of code to "unstyle" any numbers that may be linked.  The second block gives you a class to apply with a span tag to the numbers you would like linked and styled.
        //115. 
        //116.Inspired by Campaign Monitor's article on using phone numbers in email: http://www.campaignmonitor.com/blog/post/3571/using-phone-numbers-in-html-email/.
        //117. 
        //118.Step 1 (Step 2: line 224)
        //119.*/
        //120.a[href^="tel"], a[href^="sms"] {
        //121.text-decoration: none;
        //122.color: black; /* or whatever your want */
        //123.pointer-events: none;
        //124.cursor: default;
        //125.}
        //126. 
        //127..mobile_link a[href^="tel"], .mobile_link a[href^="sms"] {
        //128.text-decoration: default;
        //129.color: orange !important; /* or whatever your want */
        //130.pointer-events: auto;
        //131.cursor: default;
        //132.}
        //133.}
        //134. 
        //135./* More Specific Targeting */
        //136. 
        //137.@media only screen and (min-device-width: 768px) and (max-device-width: 1024px) {
        //138./* You guessed it, ipad (tablets, smaller screens, etc) */
        //139. 
        //140./* Step 1a: Repeating for the iPad */
        //141.a[href^="tel"], a[href^="sms"] {
        //142.text-decoration: none;
        //143.color: blue; /* or whatever your want */
        //144.pointer-events: none;
        //145.cursor: default;
        //146.}
        //147. 
        //148..mobile_link a[href^="tel"], .mobile_link a[href^="sms"] {
        //149.text-decoration: default;
        //150.color: orange !important;
        //151.pointer-events: auto;
        //152.cursor: default;
        //153.}
        //154.}
        //155. 
        //156.@media only screen and (-webkit-min-device-pixel-ratio: 2) {
        //157./* Put your iPhone 4g styles in here */
        //158.}
        //159. 
        //160./* Following Android targeting from:
        //161.http://developer.android.com/guide/webapps/targeting.html
        //162.http://pugetworks.com/2011/04/css-media-queries-for-targeting-different-mobile-devices/ ; */
        //163. 
        //164.@media only screen and (-webkit-device-pixel-ratio:.75){
        //165./* Put CSS for low density (ldpi) Android layouts in here */
        //166.}
        //167. 
        //168.@media only screen and (-webkit-device-pixel-ratio:1){
        //169./* Put CSS for medium density (mdpi) Android layouts in here */
        //170.}
        //171. 
        //172.@media only screen and (-webkit-device-pixel-ratio:1.5){
        //173./* Put CSS for high density (hdpi) Android layouts in here */
        //174.}
        //175./* end Android targeting */  


                "</style>" +

                //<!-- Targeting Windows Mobile -->
                "<!--[if IEMobile 7]>" + 
                "<style type=\"text/css\">" + 
                "</style>" + 
                "<![endif]-->" + 

                "<!--[if gte mso 9]>" + 
                "<style>" + 
                /* Target Outlook 2007 and 2010 */
                "</style>" +
                "<![endif]-->" + 

                "</head>" +
                "<body style=\"font-family: Verdana, Arial, Helvetica, sans-serif; font-size: 12px; color:#333333;\">" +

                //<!-- Wrapper/Container Table: Use a wrapper table to control the width and the background color consistently of your email. Use this approach instead of setting attributes on the body tag. -->
                "<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" id=\"backgroundTable\">" +
                "<tr><td style=\"font-family: Verdana, Arial, Helvetica, sans-serif; font-size: 11px; color:#333333;\"><center>" + 
                body + 
                "</center></td></tr>" +
                "</table>";

            wrapper_V2 += 
                "</body>" +
                "</html>" + 
                "";

    return wrapper_V2;


            //string wrapper =  
            //    "<!DOCTYPE html PUBLIC \" -//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">"
            //        + "<html>\n"
            //        + "<head>\n"
            //        + "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" />\n"
            //        + "<title>{0}</title>\n"
            //        + "</head>\n"
            //        + "<body style=\"font-family: Verdana, Arial, Helvetica, sans-serif; font-size: 12px; color:#666666;\" leftmargin=\"0\" marginwidth=\"0\" topmargin=\"0\" marginheight=\"0\" offset=\"0\">\n"
            //        + "<center> \n"
            //        + "{1}"
            //        + "<img src=\"{2}\" border=\"0\" height=\"0\" width=\"0\">"
            //        + "</center>"
            //        + "</body>\n"
            //        + "</html>\n";

            //return string.Format(wrapper,
            //        subject, 
            //        body,
            //        baseUrlForPublicSite + "counter.aspx?counterrcpid");

        }



    }

}
