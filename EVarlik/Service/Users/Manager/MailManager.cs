using System;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using EVarlik.Dto.Users;
using Newtonsoft.Json;

namespace EVarlik.Service.Users.Manager
{
    public class MailManager
    {
        public void _SendMail(UserDto userDto)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Port = 25;
                smtpClient.Host = "gator3200.hostgator.com";
                smtpClient.EnableSsl = true;
                smtpClient.Timeout = 1000;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential("bilgi@evarlik.com", "2766748.!");

                MailMessage mm = new MailMessage("bilgi@evarlik.com",
                    userDto.Mail,
                    "E-Varlik Şifre Yenileme Talebiniz",
                    userDto.Password);
                mm.BodyEncoding = Encoding.UTF8;
                mm.IsBodyHtml = true;
                mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                smtpClient.Send(mm);
            }
            catch (Exception e)
            {
            }
        }

        public void SendWelcome(UserDto userDto)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri("http://www.acgelektronik.com/evarlikMailSend.php?token=ae7635abfe63772042f1965d50f07d09");

                    var mailDto = new MailDto()
                    {
                        Body = WelcomeMailTemplate,
                        Name = userDto.Name,
                        Subject = "E-Varlik Hoş Gelidiniz",
                        To = userDto.Mail
                    };

                    var clientDtoJson = JsonConvert.SerializeObject(mailDto);
                    var content = new StringContent(clientDtoJson, Encoding.UTF8, "application/json");

                    var responseMessageR = client
                        .PostAsync("evarlikMailSend.php?token=ae7635abfe63772042f1965d50f07d09", content)
                        .ContinueWith((postTask) => postTask.Result.EnsureSuccessStatusCode());
                    responseMessageR.Wait();
                    HttpResponseMessage responseMessage = responseMessageR.Result;
                    var resR = responseMessage.Content.ReadAsStringAsync();
                    resR.Wait();
                    var res = resR.Result;
                }
                catch (Exception e)
                {
                }
            }
        }

        public void SendForgotPassword(UserDto userDto)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri("http://www.acgelektronik.com/evarlikMailSend.php?token=ae7635abfe63772042f1965d50f07d09");

                    var temmplate = ForgotPasswordTemplate.Replace("__name__", userDto.Name);
                    temmplate = temmplate.Replace("__surname__", userDto.Surname);
                    temmplate = temmplate.Replace("__password__", userDto.Password);

                    var mailDto = new MailDto()
                    {
                        Body = temmplate,
                        Name = userDto.Name,
                        Subject = "E-Varlik Şifre Yenileme Talebi",
                        To = userDto.Mail
                    };

                    var clientDtoJson = JsonConvert.SerializeObject(mailDto);
                    var content = new StringContent(clientDtoJson, Encoding.UTF8, "application/json");

                    var responseMessageR = client
                        .PostAsync("evarlikMailSend.php?token=ae7635abfe63772042f1965d50f07d09", content)
                        .ContinueWith((postTask) => postTask.Result.EnsureSuccessStatusCode());
                    responseMessageR.Wait();
                    HttpResponseMessage responseMessage = responseMessageR.Result;
                    var resR = responseMessage.Content.ReadAsStringAsync();
                    resR.Wait();
                    var res = resR.Result;
                }
                catch (Exception e)
                {
                }
            }
        }

        public void Send(UserDto userDto)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri("http://www.acgelektronik.com/evarlikMailSend.php?token=ae7635abfe63772042f1965d50f07d09");

                    var mailDto = new MailDto()
                    {
                        Body = userDto.Password,
                        Name = userDto.Name,
                        Subject = "E-Varlik Şifre Yenileme Talebiniz",
                        To = userDto.Mail
                    };

                    var clientDtoJson = JsonConvert.SerializeObject(mailDto);
                    var content = new StringContent(clientDtoJson, Encoding.UTF8, "application/json");

                    var responseMessageR = client
                        .PostAsync("evarlikMailSend.php?token=ae7635abfe63772042f1965d50f07d09", content)
                        .ContinueWith((postTask) => postTask.Result.EnsureSuccessStatusCode());
                    responseMessageR.Wait();
                    HttpResponseMessage responseMessage = responseMessageR.Result;
                    var resR = responseMessage.Content.ReadAsStringAsync();
                    resR.Wait();
                    var res = resR.Result;
                }
                catch (Exception e)
                {
                }
            }
        }

        private static readonly string ForgotPasswordTemplate = @"
<!doctype html>
<html>
  <head>
    <meta name=""viewport"" content=""width=device-width"" />
    <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
    <title>Evarlik.com Sifre Yenileme Talebi</title>
 <style>

 img {
        border: none;
        -ms-interpolation-mode: bicubic;
        max-width: 100%; }
      body {
        background-color: #f6f6f6;
        -webkit-font-smoothing: antialiased;
        font-size: 14px;
        line-height: 1.4;
        margin: 0;
        padding: 0;
        -ms-text-size-adjust: 100%;
        -webkit-text-size-adjust: 100%; }

      table {
        border-collapse: separate;
        mso-table-lspace: 0pt;
        mso-table-rspace: 0pt;
        width: 100%; }
        table td {
        font-family: Helvetica, Arial, Verdana, Trebuchet MS;
          font-size: 14px;
          vertical-align: top; }


      .body {
        background-color: #f6f6f6;
        width: 100%; }

      .container {
        display: block;
        Margin: 0 auto !important;
        max-width: 1000px;
		min-width: 280px;
		}

      .content {
        box-sizing: border-box;
        display: block;
        Margin: 0 auto;
        max-width: 1000px;
        padding: 10px 0; }
      /* -------------------------------------
       Eposta tepe
      ------------------------------------- */
	.header {background:#85bb65; height:30px; padding:10px; }
	.header_logo>h1 {font-size:24px; padding:0 10px 0 10px; line-height:35px; color:#fff; font-weight:bold; line-height:60px;}
	.hr>hr {margin:0; border: 0; border-bottom: 1px solid #b5d6a2;}
	
	
	@media only screen and (max-width: 600px) {
	.header {height:30px; }
	.header>td {padding:5px;}


	  }
	
	@media only screen and (max-width: 400px) {
	.header {height:30px;}
	.header>td {padding:5px;}

	  }
	  
	/* -------------------------------------
       Eposta anasayfa
      ------------------------------------- */
      .main {
        background: #ededed;
        border-radius: 10px;
        width: 100%; }

      .wrapper {
        box-sizing: border-box;
        padding: 20px; }

      .content-block {
        padding-bottom: 10px;
        padding-top: 10px;
      }
      /* -------------------------------------
       Eposta taban
      ------------------------------------- */
      .footer {
        clear: both;
        Margin-top: 10px;
        text-align: center;
        width: 100%; }
        .footer td,
        .footer p,
        .footer span,
        .footer a {
          color: #999999;
          font-size: 12px;
          text-align: center; }
		  
      h1 {margin:0; display:inline-block;}
      /* -------------------------------------
          buton
      ------------------------------------- */
      .btn {
        box-sizing: border-box;
        width: 100%; }
        .btn > tbody > tr > td {
          padding-bottom: 15px; }
        .btn table {
          width: auto; }
        .btn table td {
          background-color: #ffffff;
          border-radius: 5px;
          text-align: center; }
        .btn a {
          background-color: #ffffff;
          border: solid 1px #3498db;
          border-radius: 5px;
          box-sizing: border-box;
          color: #3498db;
          cursor: pointer;
          display: inline-block;
          font-size: 14px;
          font-weight: bold;
          margin: 0;
          padding: 12px 25px;
          text-decoration: none;
          text-transform: capitalize; }

      .btn-primary table td {
        background-color: #3498db; }

      .btn-primary a {
        background-color: #3498db;
        border-color: #3498db;
        color: #ffffff; }

      .preheader {
        color: transparent;
        display: none;
        height: 0;
        max-height: 0;
        max-width: 0;
        opacity: 0;
        overflow: hidden;
        mso-hide: all;
        visibility: hidden;
        width: 0; }

    </style>
  </head>
  <body class="""">
    <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""body"">
      <tr>
        <td>&nbsp;</td>
        <td class=""container"">
          <div class=""content"">

            <!-- START CENTERED WHITE CONTAINER -->
            <span class=""preheader"">Evarlik.com Yeni Şifre Talebiniz</span>
            <table class=""main"">
<tr class=""header""><td><div><div class=""header_logo""><h1>Evarlık<label>.com</label></h1></div><div class=""hr""><hr></div></tr>
              <!-- START MAIN CONTENT AREA -->
              <tr>
                <td class=""wrapper"">
                  <table border=""0"" cellpadding=""0"" cellspacing=""0"">
                    <tr>
                      <td>
                        <p>Merhaba, __name__ __surname__</p>
                        <p>Yeni şifre talebini aldık. Aşağıda yeni şifreni bulabilirsin.</p>
                        <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""btn btn-primary"">
                          <tbody>
                            <tr>
                              <td align=""center"">
                                <table border=""0"" cellpadding=""0"" cellspacing=""0"">
                                  <tbody>
                                    <tr>
                                      <td> <a>__password__</a> </td>
                                    </tr>
                                  </tbody>
                                </table>
                              </td>
                            </tr>
                          </tbody>
                        </table>
                        <p>Otomatik olarak üretilmiş bu şifreyi ilk girişinizde hesabınızın güvenliği için değiştirmenizi öneririz.</p>
                        <p>Bol Kazançlar...</p>
                      </td>
                    </tr>
                  </table>
                </td>
              </tr>

            <!-- END MAIN CONTENT AREA -->
            </table>

            <!-- START FOOTER -->
            <div class=""footer"">
              <table border=""0"" cellpadding=""0"" cellspacing=""0"">
                <tr>
                  <td class=""content-block"">
                    <span class=""apple-link"">Evarlik.com Ekibi</span>
                    <br> Bu epostanın size gelmediğini düşünüyorsanız <a href=""https://www.evarlik.com/yanlis_eposta"">rapor etmek için tıklayın</a>.
                  </td>
                </tr>
              </table>
            </div>
            <!-- END FOOTER -->

          <!-- END CENTERED WHITE CONTAINER -->
          </div>
        </td>
        <td>&nbsp;</td>
      </tr>
    </table>
  </body>
</html>
";

        private static readonly string WelcomeMailTemplate = @"
<!doctype html>
<html>
  <head>
    <meta name=""viewport"" content=""width=device-width"" />
    <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
    <title>Evarlik Hoşgeldiniz Email template</title>
    <style>
	        img {
        border: none;
        -ms-interpolation-mode: bicubic;
        max-width: 100%; }
      body {
        background-color: #f6f6f6;
        -webkit-font-smoothing: antialiased;
        font-size: 14px;
        line-height: 1.4;
        margin: 0;
        padding: 0;
        -ms-text-size-adjust: 100%;
        -webkit-text-size-adjust: 100%; }

      table {
        border-collapse: separate;
        mso-table-lspace: 0pt;
        mso-table-rspace: 0pt;
        width: 100%; }
        table td {
        font-family: Helvetica, Arial, Verdana, Trebuchet MS;
          font-size: 14px;
          vertical-align: top; }


      .body {
        background-color: #f6f6f6;
        width: 100%; }

      .container {
        display: block;
        Margin: 0 auto !important;
        max-width: 1000px;
		min-width: 280px;
		}

      .content {
        box-sizing: border-box;
        display: block;
        Margin: 0 auto;
        max-width: 1000px;
        padding: 10px 0; }
      /* -------------------------------------
       Eposta tepe
      ------------------------------------- */
	.header {background:#85bb65; height:370px; padding:10px; }
	.header_logo>h1 {padding:0 0px 0 20px; line-height:80px; font-family: Georgia;}
	.header_logo>h1 {color:#fff; font-weight:bold; font-size:48px;}
	.header_logo>h1>label {font-size:24px;}
	.header_logo>span {float:right; line-height:80px; font-size:24px; font-weight:bold; color:#fff; margin-right:10px;}
	.header>td {padding:10px;}
	.hr>hr {margin:0 -10px; border: 0; border-bottom: 1px solid #b5d6a2;}
	.coinler { margin:40px 0 0 0; width:100%; text-align:center;}
	.coinler>img {width:70px; margin:0 15px;}
	.ametin{text-align:center; margin:20px 0 0 0;}
	.ametin>h1 {display:block; color:#fff;  margin-bottom:15px;}
	
	@media only screen and (max-width: 600px) {
	.header {height:230px; }
	.header>td {padding:5px;}
	.hr>hr {margin:0 -5px; }
	.header_logo {}
	.header_logo>h1 {font-size:24px; padding:0 10px 0 10px; line-height:35px;}
	.header_logo>h1>label {font-size:16px;}
	.header_logo>span {font-size:14px; line-height:35px;}
	.wrapper {padding: 10px; }
	.content-block {padding-bottom: 5px 0;}
	.coinler { margin:30px 0 0 0; }
	.coinler>img {width:45px; margin:0 10px;}
	.ametin{margin:10px 0 0 0;}
	.ametin>h1{font-size:18px; margin-bottom:10px}

	  }
	
	@media only screen and (max-width: 400px) {
	.header {height:200px;}
	.header>td {padding:5px;}
	.hr>hr {margin:0 -5px; }
	.header_logo {}
	.header_logo>h1 {font-size:24px; padding:0 10px 0 10px; line-height:35px;}
	.header_logo>h1>label {font-size:16px;}
	.header_logo>span {font-size:14px; line-height:35px;}
	.wrapper {padding: 10px; }
	.content-block {padding-bottom: 5px 0;}
	.coinler { margin:30px 0 0 0; }
	.coinler>img {width:30px; margin:0 10px;}
	.ametin{margin:10px;}
	.ametin>h1{font-size:16px; margin-bottom:10px;}
	  }
	  
	/* -------------------------------------
       Eposta anasayfa
      ------------------------------------- */
      .main {
        background: #ffffff;
        border-radius: 10px;
        width: 100%; }

      .wrapper {
        box-sizing: border-box;
        padding: 20px; }

      .content-block {
        padding-bottom: 10px;
        padding-top: 10px;
      }
      /* -------------------------------------
       Eposta taban
      ------------------------------------- */
      .footer {
        clear: both;
        Margin-top: 10px;
        text-align: center;
        width: 100%; }
        .footer td,
        .footer p,
        .footer span,
        .footer a {
          color: #999999;
          font-size: 12px;
          text-align: center; }
		  
      h1 {margin:0; display:inline-block;}
      /* -------------------------------------
          buton
      ------------------------------------- */
      .btn {
        box-sizing: border-box;
        width: 100%; }
        .btn > tbody > tr > td {
          padding-bottom: 15px; }
        .btn table {
          width: auto; }
        .btn table td {
          background-color: #ffffff;
          border-radius: 5px;
          text-align: center; }
        .btn a {
          background-color: #ffffff;
          border: solid 1px #3498db;
          border-radius: 5px;
          box-sizing: border-box;
          color: #3498db;
          cursor: pointer;
          display: inline-block;
          font-size: 14px;
          font-weight: bold;
          margin: 0;
          padding: 12px 25px;
          text-decoration: none;
          text-transform: capitalize; }

      .btn-primary table td {
        background-color: #3498db; }

      .btn-primary a {
        background-color: #3498db;
        border-color: #3498db;
        color: #ffffff; }

      .preheader {
        color: transparent;
        display: none;
        height: 0;
        max-height: 0;
        max-width: 0;
        opacity: 0;
        overflow: hidden;
        mso-hide: all;
        visibility: hidden;
        width: 0; }

    </style>
  </head>
  <body class="""">
    <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""body"">
      <tr>
        <td>&nbsp;</td>
        <td class=""container"">
          <div class=""content"">

            <!-- START CENTERED WHITE CONTAINER -->
            <span class=""preheader"">Evarlik.com'a Hoşgeldiniz. Hemen Kazanmaya Başlayın!</span>
            <table class=""main"">
			<tr class=""header""><td><div><div class=""header_logo""><h1>Evarlık<label>.com</label></h1><span>Hoşgeldiniz</span></div><div class=""hr""><hr></div>
			<div><!-- baslik ana icerik -->
			<div class=""coinler""><img src=""data:image/png;base64,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"">
			
			<img src=""data:image/png;base64,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"">
			
			<img src=""data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAIAAAACACAMAAAD04JH5AAAAAXNSR0IArs4c6QAAAnlQTFRFAAAAAK/vAKzoAKvnAKvkAKvlAKrlAKvlAKrkAKrlAKvlAKrkAKrqAKvnAKvlAKvlAKvlAKr/AKroAKrmAKrlAKvlAK3oAKvlAKrkAK/nAKvkAKrlAKzmAKrlAKvlAKvmAKvlAKvkAK3tAKrkAL//AKvlAKrlAK3mAKvlAKvlALDlAKrlAKrnAP//AKvlAKrlALHrAKrlAK7oAKrlAKrnAK3mAKvkAKrlAKrkAKzkAKvkAKrk////Aark+Pz+B6zk6/j8/v7+7vn97/n9edLwv+n4WsjttOb3AqrkMrrpYcru6ff8/f7+uef30/D6GLLm5vb8Cq3la83vxOv4Da7l+/3+hNbxmNz0jtnz3vT7Z8zuU8Xsze75F7HmQb/qd9Hwo+D1GrLmFrHmldvzS8PrGbLm/P7+TcPsNrzpV8ftOLzpFbHms+X24PT71vH6qOL1C63lyu353/T7bc7vIbXne9PxhdbyfdPx2vL7L7nob8/vjNjyk9vzM7vpmt30VMbstub3G7PmhtbyruT2ye35t+f3z+/5od/1+f3+JbbnW8jt5Pb8Kbfog9XxSsLrxuz4BavkddHwILTnK7jocM/v4fX7PL7q8vr9qeL1bs7vven40O/65/f8Or3q7fn9TMPsBqzkXsnt5fb8ktrz0e/6pOD1Nbvp1fH69Pv9Mbrpj9nz+v3+DK7litjyyOz5nd70ctDwSMLrLrnoCKzk2PL6Pr7q1/H6VsbtZMvuvOj3puH19fv9gNTxgdXx4/X8n9/0m930WcftScLrbM7vuOf3Krjo8/v9ftTxBKvk2/P7eNLwE7DmO73qPb7qcc/vu+j3MLrppeH1uuj36enSyAAAADt0Uk5TABA3XoWpuMXS3+z5DEmLwukGQoTG+ziV7SB82TSv/j25QxyrBHTzPtSIHcRLAomoDbsWzBUf2nWsX4YZU1BUAAAEwUlEQVR4Xs3bBVsjSRAG4EaChMXD4rPBdWGB2yBf4b7u7u5+7u7u7u7u7i6/6B6WI9nAdGemZoa+9xdMMlLSVYIlLj4h0ZeUnJLqT0vzp6YkJ/kSE+LjxKyYk56RmQVTWZkZ6XOEl7JzcgN5UMoL5OZkC2/MzS+AJQX5c4XrCouKYUNxUaFwU0mpAZuM0hLhlnlBsATnCTeUZYIts0w4VV4BRyrKhROVVdVwqLqqUrDV1MIFtTXcn19nwBVGHetPqG+AaxoYT0KjHy7yNwp75jcZcJXRNN9W1GmG65ptxKgFLfBAywJhUWsbPNF2nrBkYQgeCS209PtD8Eyo1cL9b4OH2mI+B9kt8FRLtlCa3wyPNau/B03wXJNQaDTAt/zp57d80TPw4tLzL++GnNGoiD9+sB0apYiNm3oh5a+Xxt8GcO28iqLdugdSDbLoXAeu/X00w2W9kKmT5D8GmBb1k4nrpVdg1JjegFow3bKETI0+BYlas5vQDqaXXyWJ1yHTbpJ/V4OndxnJLPkdEtUzc7QOMF1IcheNQaJDTFMGppV9pHAlZMpEtEwwrSKV6yCTKaLMA9dqUroZMtGVaxBMa9aS0ibIBKPqf3DtIbWHIFUiIkrB9TCpPQqpUhFWaIBrHak9Aykj0sUpAtt6UuuBXJGYEgLbBlIbhFxxuP8GviFSew8KU928fPBtJrVhKOSLs7ILwLeV1C6AQsFkjp4DJ5aR0jao5IgJuXBiO6kM9EIlV0wIIIbuiy+5dMe1V6wb2YWZrl5LCtdAKXC2/54HpTO7F9N/+m8wSbdvJLmb9kIpb6K7nw6VXTv20blWrOpGtNtI7nbEkC6EyIDCHYtpuqXLEe1OeSC4CzFkxEhF7iYTG++xGI8G70WYIi3JgtR9ZOr+nTjXA2TuwQOIKUuIOEgdJIlHxhCBcTL12OOwIE7EQ2bvEyQzhAg8SWZGD8OKeNEJmSMktfYQIp6lmZ47CGsSRBckXiCFo1sxZe9LFG3JsVe2dcOiLuGDxHFS2fIaJp14g8L63nxr5O133oUNPmk+fHgfKZ18HxPGP6Cw3SthW1AaCT6kWIa3fzT0MYX1bwZDQKQwkj1T68GRIlJh7iTZM7AGHKnCD3NHyZ5PwOIXaTD3KdnzGVjSXLuAz7kX4Ie5U2TPl2Dxu/YQrhgDR6p7r+Ei5muYzP0Q9Xz19Tfffkdhg+NgSBZJ3E/x95hwYAVN+eHHE7AtiRuMhn/CpDM/U9ji47/YDkbMcHzqV0z5jWaE4w3Ww3GiSHCckKx2lpA4T8lOk5kjVlMy50npiKOkVJmW/2EpLd/vKC3XX5g4L83+dFaaOS9O+50Up9rLc2sNir+8a1Dob9Hob1Lpb9Ppb1Tqb9WKYr3Nav3tev0HFqIUXKddOLLRf2il/9hO/8Gl/qNb/YfXomJ2j+8r9A8wzFA1myMcVdqHWHSP8egeZNI+yqV7mM2zcb6/J8b5jv3DGOfTMNCocaRT91Cr7rFe3YPNuke7NQ+3t+oe79e94KB7xUPzkovmNZ963YtOule92CrbnS+7tVcKJ8o74EhHueaFR80rn5qXXl1e+w3BhlBRod7FZ82r39qX370XF9/Z5QsGptb/A0FfVydz/f9fQqjopOS5h+sAAAAASUVORK5CYII="">
			
			<img src=""data:image/png;base64,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"">
			
			<img src=""data:image/png;base64,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"">
			
			</div>
			<div class=""ametin""><h1>EN İYİLER BURADA !</h1>
			<table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""btn btn-primary"">
                          <tbody>
                            <tr>
                              <td align=""center"">
                                <table border=""0"" cellpadding=""0"" cellspacing=""0"">
                                  <tbody>
                                    <tr>
                                      <td> <a href=""https://www.evarlik.com"" target=""_blank"">Hemen Al/Sat Yap</a> </td>
                                    </tr>
                                  </tbody>
                                </table>
                              </td>
                            </tr>
                          </tbody>
             </table>
			</div>
			</div><!-- baslik ana icerik son-->
			</div></td></tr>
              
              <tr>
                <td class=""wrapper"">
                  <table border=""0"" cellpadding=""0"" cellspacing=""0"">
                    <tr>
                      <td>
                        <p>Evarlik.com'a Hoş geldiniz</p>
                        <p>Dünyada hızla popülerleşen dijital para, şimdiye kadar hızlı değer artışı ile pek kişiye inanılmaz kazançlar* sağladı.</p> <p>Bu hızlı değişime Microsoft ve Virgin Galactic gibi büyük firmalar, bitcoin'i ödeme olarak kabul ederek başladılar. Bunun anlamı bütün dünyaya çok az komisyonlarla kolayca para gönderebilecek ve ürün alabileceksiniz.</p><p> Banknotlarımızın yerini bu dijital paralar almadan önce siz de evarlik.com kolaylığı ile dijital para dünyasında yerinizi alın</p>
						<p>Sorularınızı <b>destek@evarlik.com</b> elektronik posta adresine gönderebilirsiniz. Size kısa sürede cevap vereceğiz.</p>
                        <p>Bol Kazançlar...</p>
                      </td>
                    </tr>
                  </table>
                </td>
              </tr>

            <!-- END MAIN CONTENT AREA -->
            </table>

            <!-- START FOOTER -->
            <div class=""footer"">
              <table border=""0"" cellpadding=""0"" cellspacing=""0"">
                <tr>
                  <td class=""content-block"">
                    <span class=""apple-link"">* Yatırım tavsiyesi değildir. Dijital para birimlerine yatırım işlemleri belli bir bilgi birikimi gerekebilir. Yatırımlarınızı dikkatli ve bilinçli yapmanız tavsiye edilir.</span>
                  </td>
                </tr>
                <tr>
                </tr>
              </table>
            </div>
            <!-- END FOOTER -->

          <!-- END CENTERED WHITE CONTAINER -->
          </div>
        </td>
        <td>&nbsp;</td>
      </tr>
    </table>
  </body>
</html>
";

    }

    public class MailDto
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string To { get; set; }
        public string Name { get; set; }
    }
}