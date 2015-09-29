using System.Text;
using System.Text.RegularExpressions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace ITJakub.MobileApps.Client.Core.Manager.Authentication.LocalAuthentication
{
    public class LocalAuthViewModel : ViewModelBase
    {
        private string m_textError;
        private bool m_isError;
        private bool m_showLoginControls;
        private bool m_showCreateControls;

        public LocalAuthViewModel()
        {
            CreateUserCommand = new RelayCommand(CreateUser);
            LoginCommand = new RelayCommand(Login);
            CancelCommand = new RelayCommand(Cancel);
            SubmitCommand = new RelayCommand(() =>
            {
                if (ShowLoginControls)
                    Login();
                else
                    CreateUser();
            });
        }
        
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string PasswordConfirm { get; set; }

        public RelayCommand CreateUserCommand { get; private set; }

        public RelayCommand LoginCommand { get; private set; }

        public RelayCommand SubmitCommand { get; private set; }

        public RelayCommand CancelCommand { get; private set; }

        public string TextError
        {
            get { return m_textError; }
            set
            {
                m_textError = value;
                RaisePropertyChanged();
            }
        }

        public bool IsError
        {
            get { return m_isError; }
            set
            {
                m_isError = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowLoginControls
        {
            get { return m_showLoginControls; }
            set
            {
                m_showLoginControls = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowCreateControls
        {
            get { return m_showCreateControls; }
            set
            {
                m_showCreateControls = value;
                RaisePropertyChanged();
            }
        }
        
        private void Login()
        {
            IsError = false;
            CheckLoginInputs();

            if (!IsError)
                SubmitForm();
        }

        private void CreateUser()
        {
            IsError = false;
            CheckCreateUserInputs();

            if (!IsError)
                SubmitForm();
        }

        private void SubmitForm()
        {
            var userLoginSkeleton = new UserLoginSkeletonWithPassword
            {
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
                Password = Password,
                Success = true
            };

            Messenger.Default.Send(new LocalAuthCompletedMessage
            {
                UserLoginSkeleton = userLoginSkeleton
            });
        }

        private void Cancel()
        {
            Messenger.Default.Send(new LocalAuthCompletedMessage
            {
                UserLoginSkeleton = new UserLoginSkeletonWithPassword { Success = false }
            });
        }

        private void CheckLoginInputs()
        {
            IsError = false;
            var errorStringBuilder = new StringBuilder();

            if (string.IsNullOrWhiteSpace(Email))
            {
                IsError = true;
                errorStringBuilder.AppendLine("Zadejte e-mail");
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                IsError = true;
                errorStringBuilder.AppendLine("Zadejte heslo");
            }

            if (IsError)
            {
                TextError = errorStringBuilder.ToString();
            }
        }

        private void CheckCreateUserInputs()
        {
            IsError = false;
            var errorStringBuilder = new StringBuilder();

            if (string.IsNullOrWhiteSpace(FirstName))
            {
                IsError = true;
                errorStringBuilder.AppendLine("Zadejte křestní jméno");
            }

            if (string.IsNullOrWhiteSpace(LastName))
            {
                IsError = true;
                errorStringBuilder.AppendLine("Zadejte příjmení");
            }

            if (string.IsNullOrWhiteSpace(Email) || !Regex.IsMatch(Email, @"^.+@.+\..+$"))
            {
                IsError = true;
                errorStringBuilder.AppendLine("Zadejte e-mail ve formátu email@example.com (bude sloužit pro přihlašování)");
            }

            if (string.IsNullOrWhiteSpace(Password) || Password.Length < 6)
            {
                IsError = true;
                errorStringBuilder.AppendLine("Zadejte heslo dlouhé alespoň 6 znaků");
            }

            if (Password != PasswordConfirm)
            {
                IsError = true;
                errorStringBuilder.AppendLine("Zadané heslo a jeho kontrola se neshodují");
            }

            if (IsError)
            {
                TextError = errorStringBuilder.ToString();
            }
        }

        public void ShowAuthenticationError()
        {
            IsError = true;
            TextError = ShowLoginControls ? "Nesprávný e-mail nebo heslo\n" : "Uživatel s tímto e-mailem již existuje\n";
        }
    }
}
