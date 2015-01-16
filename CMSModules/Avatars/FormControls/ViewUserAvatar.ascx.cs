using System;

using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.SettingsProvider;
using CMS.CMSHelper;

public partial class CMSModules_Avatars_FormControls_ViewUserAvatar : FormEngineUserControl
{
    private int mValue;
    private int mMaxSideSize = 200;


    #region "Public properties"

    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return mValue;
        }
        set
        {
            // get avatar id
            mValue = ValidationHelper.GetInteger(value, 0);
        }
    }


    /// <summary>
    /// Gets or sets max side size of avatar image.
    /// </summary>
    public int MaxSideSize
    {
        get
        {
            return mMaxSideSize;
        }
        set
        {
            mMaxSideSize = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        AvatarInfo ai = null;
        int userID = 0;
        UserInfo userInfo = null;

        // Load avatar type setting
        string aType = DataHelper.GetNotEmpty(SettingsKeyProvider.GetStringValue(CMSContext.CurrentSiteName + ".CMSAvatarType"), AvatarInfoProvider.AVATAR);

        if (aType == AvatarInfoProvider.USERCHOICE)
        {
            // Get userID from FormEngine
            if (this.Data != null)
            {
                userID = ValidationHelper.GetInteger(this.Data["UserID"], 0);
            }

            userInfo = UserInfoProvider.GetUserInfo(userID);

            // Set user avatar type
            if (userInfo != null)
            {
                aType = userInfo.UserSettings.UserAvatarType;
            }
            else
            {
                aType = AvatarInfoProvider.GRAVATAR;
            }
        }

        switch (aType)
        {
            case AvatarInfoProvider.AVATAR:

                if (mValue > 0)
                {
                    ai = AvatarInfoProvider.GetAvatarInfo(mValue);
                }

                if (ai == null)
                {
                    DefaultAvatarTypeEnum defAvatar = DefaultAvatarTypeEnum.User;
                    UserGenderEnum gender = (UserGenderEnum)ValidationHelper.GetInteger(Form.GetFieldValue("UserGender"), 0);
                    switch (gender)
                    {
                        case UserGenderEnum.Female:
                            defAvatar = DefaultAvatarTypeEnum.Female;
                            break;

                        case UserGenderEnum.Male:
                            defAvatar = DefaultAvatarTypeEnum.Male;
                            break;
                    }

                    ai = AvatarInfoProvider.GetDefaultAvatar(defAvatar);

                    // Avatar not specified for current gender, get user default avatar
                    if (ai == null)
                    {
                        ai = AvatarInfoProvider.GetDefaultAvatar(DefaultAvatarTypeEnum.User);
                    }
                }

                if (ai != null)
                {
                    imgAvatar.ImageUrl = ResolveUrl("~/CMSModules/Avatars/CMSPages/GetAvatar.aspx?avatarguid=" + ai.AvatarGUID + "&maxsidesize=" + MaxSideSize);
                    imgAvatar.AlternateText = GetString("general.avatar");
                    return;
                }

                imgAvatar.Visible = false;
                break;
            case AvatarInfoProvider.GRAVATAR:

                if (userInfo == null)
                {
                    // Get userID from FormEngine
                    if (this.Data != null)
                    {
                        userID = ValidationHelper.GetInteger(this.Data["UserID"], 0);
                    }

                    userInfo = UserInfoProvider.GetUserInfo(userID);
                }

                if (userInfo != null)
                {
                    imgAvatar.ImageUrl = AvatarInfoProvider.CreateGravatarLink(userInfo.Email, userInfo.UserSettings.UserGender, ResolveUrl("~/CMSModules/Avatars/CMSPages/GetAvatar.aspx?maxsidesize=" + MaxSideSize + "&avatarguid="), MaxSideSize, CMSContext.CurrentSiteName);
                    imgAvatar.AlternateText = GetString("general.avatar");
                }
                else
                {
                    imgAvatar.Visible = false;
                }
                break;
        }



    }

    #endregion
}