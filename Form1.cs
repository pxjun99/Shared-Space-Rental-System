using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        // ==========================================
        // [1. 데이터 모델 정의]
        // ==========================================
        public class SpaceModel
        {
            public int Id { get; set; }
            public string Category { get; set; }
            public string CategoryName { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Amenities { get; set; }
            public int Capacity { get; set; }
            public int PricePerHour { get; set; }
            public double Rating { get; set; }
            public bool IsLiked { get; set; }
            public List<int> ReservedHours { get; set; } = new List<int>();
            public List<ReviewModel> Reviews { get; set; } = new List<ReviewModel>();
        }

        public class ReviewModel
        {
            public string Author { get; set; }
            public int Rating { get; set; }
            public string Comment { get; set; }
        }

        public class ReservationModel
        {
            public string UserName { get; set; }
            public string UserPhone { get; set; }
            public string SpaceTitle { get; set; }
            public string DateText { get; set; }
            public string TimeText { get; set; }
            public string PaymentMethod { get; set; }
            public string PriceText { get; set; }
            public int SpaceId { get; set; }
            public List<int> Hours { get; set; } = new List<int>();
        }

        // ==========================================
        // [2. 필드 및 상태 변수]
        // ==========================================
        private List<SpaceModel> _spaces = new List<SpaceModel>();
        private List<ReservationModel> _reservations = new List<ReservationModel>();
        private SpaceModel _selectedSpace = null;
        private List<int> _selectedHours = new List<int>();
        private bool _isDarkMode = false;
        private string _selectedCategoryFilter = "All";

        // 테마 색상 변수
        private Color _bgApp = Color.FromArgb(241, 245, 249);
        private Color _bgSidebar = Color.FromArgb(15, 23, 42);
        private Color _bgCard = Color.White;
        private Color _borderCard = Color.FromArgb(226, 232, 240);
        private Color _textMain = Color.FromArgb(15, 23, 42);
        private Color _textSub = Color.FromArgb(100, 116, 139);
        private Color _bgInput = Color.FromArgb(248, 250, 252);

        // UI 메인 파트 컨트롤
        private Panel _sidebarPanel;
        private Panel _mainContentPanel;
        private Panel _viewReserve;
        private Panel _viewHistory;
        private FlowLayoutPanel _cardsFlowPanel;
        private Panel _rightDetailPanel;
        private Panel _paymentModalOverlay;

        // 상단 위젯 텍스트
        private Label _lblTotalSpace;
        private Label _lblWishlist;
        private Label _lblReservationCount;

        // 상세 및 타임칩 컨트롤
        private Label _lblSelectedTitle;
        private Label _lblSelectedPrice;
        private TextBox _txtUserName;
        private TextBox _txtUserPhone;
        private DateTimePicker _datePicker;
        private FlowLayoutPanel _timeChipsPanel;
        private Label _lblTotalPrice;
        private FlowLayoutPanel _reviewsFlowPanel;
        private ComboBox _cboRating;
        private TextBox _txtReviewComment;

        // 검색/필터 컨트롤
        private ComboBox _cboCapacity;
        private TextBox _txtSearch;

        // 내 예약 관리 내역 그리드
        private DataGridView _gridHistory;

        // 모달 컨트롤
        private Label _lblModalTitle;
        private Label _lblModalTime;
        private Label _lblModalPrice;
        private RadioButton _radioKakao;
        private RadioButton _radioToss;
        private RadioButton _radioCard;

        public Form1()
        {
            this.Text = "SPACE HUB | 프리미엄 공간 대여 플랫폼";
            this.Size = new Size(1360, 880);
            this.MinimumSize = new Size(1200, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("맑은 고딕", 9.5F, FontStyle.Regular);

            InitializeDummyData();
            BuildUI();
            ApplyTheme();
            RenderSpaceList();

            // [핵심 추가] 시작하자마자 첫 번째 공간을 자동으로 선택하여 타임칩이 즉시 보이도록 설정
            if (_spaces.Count > 0)
            {
                SelectSpace(_spaces[0]);
            }
        }

        private void InitializeDummyData()
        {
            _spaces = new List<SpaceModel>
            {
                new SpaceModel { Id = 1, Category = "Office", CategoryName = "🏢 공유오피스", Title = "강남 루프탑 프라이빗 오피스", Description = "도심 전경이 한눈에 보이는 최상층 독립형 업무 공간입니다.", Amenities = "WiFi · 모니터 · 비버리지바", Capacity = 4, PricePerHour = 15000, Rating = 4.8, ReservedHours = new List<int>{ 10, 11, 14 } },
                new SpaceModel { Id = 2, Category = "Meeting", CategoryName = "🤝 회의실", Title = "성수 디자이너 콘셉트 미팅룸", Description = "초고속 와이파이와 4K 스크린이 구비된 메인 회의실입니다.", Amenities = "4K beam · 화이트보드 · 음료", Capacity = 8, PricePerHour = 25000, Rating = 4.9, ReservedHours = new List<int>{ 13, 15, 16 } },
                new SpaceModel { Id = 3, Category = "Kitchen", CategoryName = "🍳 공유주방", Title = "홍대 쿠킹 스튜디오 파티룸", Description = "최신 인덕션 및 오븐 시설이 완비된 다목적 주방입니다.", Amenities = "오븐 · 정수기 · 촬영조명", Capacity = 6, PricePerHour = 30000, Rating = 4.7, ReservedHours = new List<int>{ 18, 19 } },
                new SpaceModel { Id = 4, Category = "Office", CategoryName = "🏢 공유오피스", Title = "판교 테크노 1인 포커스 룸", Description = "조용하게 집중할 수 있는 1인 전용 몰입형 공유 오피스.", Amenities = "모션데스크 · 시디즈의자", Capacity = 1, PricePerHour = 8000, Rating = 4.6, ReservedHours = new List<int>{ 9, 10 } },
                new SpaceModel { Id = 5, Category = "Meeting", CategoryName = "🤝 회의실", Title = "여의도 프리미엄 임원 이사회실", Description = "중요한 비즈니스 미팅과 세미나에 최적화된 고급 회의실.", Amenities = "화상회의설비 · 케이터링", Capacity = 12, PricePerHour = 45000, Rating = 5.0, ReservedHours = new List<int>{ 11, 12, 13 } }
            };

            _spaces[0].Reviews.Add(new ReviewModel { Author = "김철수", Rating = 5, Comment = "채광이 좋고 집중이 아주 잘 됩니다!" });
            _spaces[0].Reviews.Add(new ReviewModel { Author = "이영희", Rating = 4, Comment = "커피 머신이 있어서 편하게 이용했어요." });
        }

        private void BuildUI()
        {
            this.Controls.Clear();

            // 1. 사이드바
            _sidebarPanel = new Panel { Dock = DockStyle.Left, Width = 210, Padding = new Padding(12) };
            BuildSidebar();

            // 2. 메인 영역
            _mainContentPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15) };

            // 상단 수치 요약 위젯
            Panel statsPanel = BuildStatsWidget();
            statsPanel.Dock = DockStyle.Top;
            statsPanel.Height = 110;

            // 중앙 작업 영역
            Panel centerContainer = new Panel { Dock = DockStyle.Fill };

            _viewReserve = BuildReserveView();
            _viewReserve.Dock = DockStyle.Fill;

            _viewHistory = BuildHistoryView();
            _viewHistory.Dock = DockStyle.Fill;
            _viewHistory.Visible = false;

            centerContainer.Controls.Add(_viewReserve);
            centerContainer.Controls.Add(_viewHistory);

            _mainContentPanel.Controls.Add(centerContainer);
            _mainContentPanel.Controls.Add(statsPanel);

            // 3. 결제 모달 팝업 패널
            _paymentModalOverlay = BuildPaymentModal();
            _paymentModalOverlay.Visible = false;

            this.Controls.Add(_paymentModalOverlay);
            this.Controls.Add(_mainContentPanel);
            this.Controls.Add(_sidebarPanel);
        }

        private void BuildSidebar()
        {
            _sidebarPanel.Controls.Clear();
            _sidebarPanel.BackColor = Color.FromArgb(15, 23, 42);

            Label logo = new Label
            {
                Text = "SPACE HUB",
                Font = new Font("맑은 고딕", 15, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 35
            };

            Label subLogo = new Label
            {
                Text = "Space Rental Service",
                Font = new Font("맑은 고딕", 8.5F),
                ForeColor = Color.FromArgb(148, 163, 184),
                Dock = DockStyle.Top,
                Height = 30
            };

            Panel menuPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 10, 0, 0)
            };

            Button btnNavReserve = CreateSidebarBtn("🔍   공간 탐색 및 대여");
            btnNavReserve.Click += (s, e) => SwitchNav(true);

            Button btnNavHistory = CreateSidebarBtn("📋   내 예약 관리 내역");
            btnNavHistory.Click += (s, e) => SwitchNav(false);

            menuPanel.Controls.Add(btnNavHistory);
            menuPanel.Controls.Add(btnNavReserve);

            Panel darkTogglePanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 45,
                BackColor = Color.Transparent,
                Padding = new Padding(5, 8, 5, 8)
            };

            Label lblDark = new Label
            {
                Text = "다크 모드",
                ForeColor = Color.White,
                Location = new Point(5, 12),
                AutoSize = true,
                Font = new Font("맑은 고딕", 9, FontStyle.Bold),
                BackColor = Color.Transparent
            };

            CheckBox chkDark = new CheckBox
            {
                Text = "적용",
                ForeColor = Color.FromArgb(226, 232, 240),
                Location = new Point(115, 10),
                AutoSize = true,
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent
            };
            chkDark.CheckedChanged += (s, e) =>
            {
                _isDarkMode = chkDark.Checked;
                ApplyTheme();
            };

            darkTogglePanel.Controls.Add(lblDark);
            darkTogglePanel.Controls.Add(chkDark);

            _sidebarPanel.Controls.Add(darkTogglePanel);
            _sidebarPanel.Controls.Add(menuPanel);
            _sidebarPanel.Controls.Add(subLogo);
            _sidebarPanel.Controls.Add(logo);
        }

        private Button CreateSidebarBtn(string text)
        {
            Button btn = new Button
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = 42,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = new Font("맑은 고딕", 9.5F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 0, 0),
                Margin = new Padding(0, 0, 0, 6),
                Cursor = Cursors.Hand,
                UseMnemonic = false
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 41, 59);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(51, 65, 85);

            return btn;
        }

        private Panel BuildStatsWidget()
        {
            TableLayoutPanel tlp = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 1,
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 0, 0, 10)
            };
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34f));

            Panel card1 = CreateStatCard("등록된 전체 공간", "5 개", Color.FromArgb(15, 23, 42), out _lblTotalSpace);
            Panel card2 = CreateStatCard("내 찜한 공간", "0 개", Color.FromArgb(225, 29, 72), out _lblWishlist);
            Panel card3 = CreateStatCard("내 누적 예약 건수", "0 건", Color.FromArgb(37, 99, 235), out _lblReservationCount);

            tlp.Controls.Add(card1, 0, 0);
            tlp.Controls.Add(card2, 1, 0);
            tlp.Controls.Add(card3, 2, 0);

            return tlp;
        }

        private Panel CreateStatCard(string title, string defaultVal, Color valColor, out Label valueLabel)
        {
            Panel p = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(4),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(12, 10, 12, 10)
            };

            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("맑은 고딕", 9F, FontStyle.Bold),
                Location = new Point(12, 10),
                AutoSize = true
            };

            valueLabel = new Label
            {
                Text = defaultVal,
                Font = new Font("맑은 고딕", 16F, FontStyle.Bold),
                ForeColor = valColor,
                Location = new Point(10, 32),
                Size = new Size(200, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };

            p.Controls.Add(lblTitle);
            p.Controls.Add(valueLabel);

            return p;
        }

        private Panel BuildReserveView()
        {
            Panel p = new Panel();

            Panel filterBar = new Panel { Dock = DockStyle.Top, Height = 42, Padding = new Padding(0, 2, 0, 8) };

            Panel rightFilter = new Panel { Dock = DockStyle.Right, Width = 280, Height = 32 };
            _cboCapacity = new ComboBox { Width = 110, DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(165, 3) };
            _cboCapacity.Items.AddRange(new object[] { "전체 인원", "1인 전용", "4인 이하", "5인 이상" });
            _cboCapacity.SelectedIndex = 0;
            _cboCapacity.SelectedIndexChanged += (s, e) => RenderSpaceList();

            _txtSearch = new TextBox { Width = 150, Location = new Point(10, 3) };
            _txtSearch.TextChanged += (s, e) => RenderSpaceList();

            rightFilter.Controls.Add(_txtSearch);
            rightFilter.Controls.Add(_cboCapacity);

            FlowLayoutPanel catPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, Height = 32, AutoSize = true };
            Button btnAll = new Button { Text = "전체", Width = 55, Height = 30, FlatStyle = FlatStyle.Flat, Tag = "All", Cursor = Cursors.Hand };
            Button btnOff = new Button { Text = "🏢 오피스", Width = 80, Height = 30, FlatStyle = FlatStyle.Flat, Tag = "Office", Cursor = Cursors.Hand };
            Button btnMeet = new Button { Text = "🤝 회의실", Width = 80, Height = 30, FlatStyle = FlatStyle.Flat, Tag = "Meeting", Cursor = Cursors.Hand };
            Button btnKit = new Button { Text = "🍳 공유주방", Width = 95, Height = 30, FlatStyle = FlatStyle.Flat, Tag = "Kitchen", Cursor = Cursors.Hand };

            btnAll.Click += CategoryFilter_Click;
            btnOff.Click += CategoryFilter_Click;
            btnMeet.Click += CategoryFilter_Click;
            btnKit.Click += CategoryFilter_Click;

            catPanel.Controls.Add(btnAll);
            catPanel.Controls.Add(btnOff);
            catPanel.Controls.Add(btnMeet);
            catPanel.Controls.Add(btnKit);

            filterBar.Controls.Add(catPanel);
            filterBar.Controls.Add(rightFilter);

            Panel contentSplit = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 5, 0, 0) };

            _rightDetailPanel = BuildDetailDashboard();
            _rightDetailPanel.Dock = DockStyle.Right;
            _rightDetailPanel.Width = 350;

            _cardsFlowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(0, 0, 10, 0)
            };

            contentSplit.Controls.Add(_cardsFlowPanel);
            contentSplit.Controls.Add(_rightDetailPanel);

            p.Controls.Add(contentSplit);
            p.Controls.Add(filterBar);

            return p;
        }

        private Panel BuildDetailDashboard()
        {
            Panel p = new Panel { Padding = new Padding(10), BorderStyle = BorderStyle.FixedSingle };

            // 헤더 영역
            Label header = new Label { Text = "공간 예약 대시보드", Font = new Font("맑은 고딕", 11, FontStyle.Bold), Dock = DockStyle.Top, Height = 25 };
            _lblSelectedTitle = new Label { Text = "공간을 선택해주세요", Font = new Font("맑은 고딕", 10F, FontStyle.Bold), ForeColor = Color.FromArgb(37, 99, 235), Dock = DockStyle.Top, Height = 22 };
            _lblSelectedPrice = new Label { Text = "0원 / 시간", Font = new Font("맑은 고딕", 9F), Dock = DockStyle.Top, Height = 20 };

            // 사용자 정보
            Panel userInfoPanel = new Panel { Dock = DockStyle.Top, Height = 30 };
            _txtUserName = new TextBox { Text = "홍길동", Width = 90, Location = new Point(0, 3) };
            _txtUserPhone = new TextBox { Text = "010-1234-5678", Width = 130, Location = new Point(95, 3) };
            userInfoPanel.Controls.Add(_txtUserName);
            userInfoPanel.Controls.Add(_txtUserPhone);

            // 날짜 선택
            _datePicker = new DateTimePicker { Dock = DockStyle.Top, Format = DateTimePickerFormat.Short, Height = 25 };
            _datePicker.ValueChanged += (s, e) => RenderTimeChips();

            // 시간대 타이틀
            Label lblTimeChipTitle = new Label { Text = "🕒 이용 시간대 선택 (회색: 예약불가)", Font = new Font("맑은 고딕", 8.5F, FontStyle.Bold), Dock = DockStyle.Top, Height = 22 };

            // [핵심 수정] 타임칩 패널의 높이 및 AutoScroll 보장
            _timeChipsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 110, // 충분한 높이 확보
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(3)
            };

            // 결제 금액 안내
            Panel totalPanel = new Panel { Dock = DockStyle.Top, Height = 30 };
            Label lblTotalText = new Label { Text = "총 결제 금액", Location = new Point(0, 6), AutoSize = true, Font = new Font("맑은 고딕", 9, FontStyle.Bold) };
            _lblTotalPrice = new Label { Text = "0원", Location = new Point(160, 3), AutoSize = true, Font = new Font("맑은 고딕", 11.5F, FontStyle.Bold), ForeColor = Color.FromArgb(225, 29, 72) };
            totalPanel.Controls.Add(lblTotalText);
            totalPanel.Controls.Add(_lblTotalPrice);

            // 결제 버튼
            Button btnPay = new Button
            {
                Text = "결제 및 예약하기",
                Dock = DockStyle.Top,
                Height = 36,
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("맑은 고딕", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnPay.FlatAppearance.BorderSize = 0;
            btnPay.Click += BtnPay_Click;

            // 후기 헤더 및 리스트
            Label lblReviewHeader = new Label { Text = "💬 실시간 방문 후기", Font = new Font("맑은 고딕", 9F, FontStyle.Bold), Dock = DockStyle.Top, Height = 24 };
            _reviewsFlowPanel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 80, AutoScroll = true };

            // 후기 작성
            Panel addReviewPanel = new Panel { Dock = DockStyle.Top, Height = 70 };
            _cboRating = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 110, Location = new Point(0, 3) };
            _cboRating.Items.AddRange(new object[] { "5점 ★★★★★", "4점 ★★★★☆", "3점 ★★★☆☆", "2점 ★★☆☆☆", "1점 ★☆☆☆☆" });
            _cboRating.SelectedIndex = 0;

            _txtReviewComment = new TextBox { Multiline = true, Width = 190, Height = 32, Location = new Point(0, 30) };

            Button btnSaveReview = new Button
            {
                Text = "후기 저장",
                Location = new Point(195, 30),
                Width = 85,
                Height = 32,
                BackColor = Color.FromArgb(71, 85, 105),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("맑은 고딕", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSaveReview.FlatAppearance.BorderSize = 0;
            btnSaveReview.Click += BtnSaveReview_Click;

            addReviewPanel.Controls.Add(_cboRating);
            addReviewPanel.Controls.Add(_txtReviewComment);
            addReviewPanel.Controls.Add(btnSaveReview);

            // 역순 추가 (Dock = DockStyle.Top 특성)
            p.Controls.Add(addReviewPanel);
            p.Controls.Add(_reviewsFlowPanel);
            p.Controls.Add(lblReviewHeader);
            p.Controls.Add(btnPay);
            p.Controls.Add(totalPanel);
            p.Controls.Add(_timeChipsPanel); // 타임칩 패널
            p.Controls.Add(lblTimeChipTitle);
            p.Controls.Add(_datePicker);
            p.Controls.Add(userInfoPanel);
            p.Controls.Add(_lblSelectedPrice);
            p.Controls.Add(_lblSelectedTitle);
            p.Controls.Add(header);

            return p;
        }

        private Panel BuildHistoryView()
        {
            Panel p = new Panel { Padding = new Padding(10) };
            Label title = new Label { Text = "내 예약 정보 관리", Font = new Font("맑은 고딕", 12, FontStyle.Bold), Dock = DockStyle.Top, Height = 35 };

            _gridHistory = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false
            };

            _gridHistory.Columns.Add("UserName", "예약자");
            _gridHistory.Columns.Add("UserPhone", "연락처");
            _gridHistory.Columns.Add("SpaceTitle", "공간명");
            _gridHistory.Columns.Add("DateText", "예약 날짜");
            _gridHistory.Columns.Add("TimeText", "이용 시간");
            _gridHistory.Columns.Add("PaymentMethod", "결제 수단");
            _gridHistory.Columns.Add("PriceText", "결제 금액");

            DataGridViewButtonColumn cancelBtnCol = new DataGridViewButtonColumn
            {
                HeaderText = "관리",
                Text = "취소",
                UseColumnTextForButtonValue = true,
                Width = 70
            };
            _gridHistory.Columns.Add(cancelBtnCol);
            _gridHistory.CellClick += GridHistory_CellClick;

            p.Controls.Add(_gridHistory);
            p.Controls.Add(title);
            return p;
        }

        private Panel BuildPaymentModal()
        {
            Panel overlay = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(160, 15, 23, 42) };

            Panel dialog = new Panel
            {
                Size = new Size(360, 360),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            overlay.Resize += (s, e) =>
            {
                dialog.Location = new Point((overlay.Width - dialog.Width) / 2, (overlay.Height - dialog.Height) / 2);
            };

            Label title = new Label { Text = "💳 SPACE HUB 안전 결제", Font = new Font("맑은 고딕", 11.5F, FontStyle.Bold), Location = new Point(20, 20), AutoSize = true };
            _lblModalTitle = new Label { Text = "공간명", Location = new Point(20, 55), AutoSize = true, Font = new Font("맑은 고딕", 9.5F, FontStyle.Bold) };
            _lblModalTime = new Label { Text = "날짜 및 시간", Location = new Point(20, 80), AutoSize = true, ForeColor = Color.Gray };
            _lblModalPrice = new Label { Text = "최종 금액: 0원", Location = new Point(20, 105), AutoSize = true, Font = new Font("맑은 고딕", 10.5F, FontStyle.Bold), ForeColor = Color.FromArgb(225, 29, 72) };

            Label lblPayType = new Label { Text = "결제 수단 선택", Location = new Point(20, 140), AutoSize = true, Font = new Font("맑은 고딕", 9F, FontStyle.Bold) };
            _radioKakao = new RadioButton { Text = "💛 카카오페이 (KakaoPay)", Location = new Point(25, 165), AutoSize = true, Checked = true };
            _radioToss = new RadioButton { Text = "💙 토스페이 (TossPay)", Location = new Point(25, 195), AutoSize = true };
            _radioCard = new RadioButton { Text = "💳 신용 / 체크카드", Location = new Point(25, 225), AutoSize = true };

            Button btnCancel = new Button { Text = "취소", Location = new Point(25, 280), Width = 130, Height = 38, BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnCancel.Click += (s, e) => _paymentModalOverlay.Visible = false;

            Button btnConfirm = new Button { Text = "승인 및 결제", Location = new Point(175, 280), Width = 145, Height = 38, BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnConfirm.Click += BtnConfirmPayment_Click;

            dialog.Controls.Add(title);
            dialog.Controls.Add(_lblModalTitle);
            dialog.Controls.Add(_lblModalTime);
            dialog.Controls.Add(_lblModalPrice);
            dialog.Controls.Add(lblPayType);
            dialog.Controls.Add(_radioKakao);
            dialog.Controls.Add(_radioToss);
            dialog.Controls.Add(_radioCard);
            dialog.Controls.Add(btnCancel);
            dialog.Controls.Add(btnConfirm);

            overlay.Controls.Add(dialog);
            return overlay;
        }

        private void ApplyTheme()
        {
            if (_isDarkMode)
            {
                _bgApp = Color.FromArgb(15, 23, 42);
                _bgSidebar = Color.FromArgb(15, 23, 42);
                _bgCard = Color.FromArgb(30, 41, 59);
                _borderCard = Color.FromArgb(51, 65, 85);
                _textMain = Color.FromArgb(248, 250, 252);
                _textSub = Color.FromArgb(148, 163, 184);
                _bgInput = Color.FromArgb(51, 65, 85);
            }
            else
            {
                _bgApp = Color.FromArgb(241, 245, 249);
                _bgSidebar = Color.FromArgb(15, 23, 42);
                _bgCard = Color.White;
                _borderCard = Color.FromArgb(226, 232, 240);
                _textMain = Color.FromArgb(15, 23, 42);
                _textSub = Color.FromArgb(100, 116, 139);
                _bgInput = Color.FromArgb(248, 250, 252);
            }

            this.BackColor = _bgApp;
            _sidebarPanel.BackColor = _bgSidebar;
            _mainContentPanel.BackColor = _bgApp;
            _rightDetailPanel.BackColor = _bgCard;
            _gridHistory.BackgroundColor = _bgCard;

            ApplyControlColors(this);
            RenderSpaceList();
            RenderTimeChips();
        }

        // [단일 선언] 중복 선언을 방지한 단 하나의 ApplyControlColors 메서드
        private void ApplyControlColors(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                // 사이드바 컨트롤 영역은 적용 예외
                if (c == _sidebarPanel || c.Parent == _sidebarPanel || (c.Parent != null && c.Parent.Parent == _sidebarPanel))
                {
                    continue;
                }

                if (c is Label lbl)
                {
                    if (lbl.ForeColor != Color.FromArgb(37, 99, 235) && lbl.ForeColor != Color.FromArgb(225, 29, 72))
                        lbl.ForeColor = _textMain;
                }
                else if (c is Panel p && p != _paymentModalOverlay)
                {
                    p.BackColor = _bgCard;
                }
                else if (c is TextBox || c is ComboBox)
                {
                    c.BackColor = _bgInput;
                    c.ForeColor = _textMain;
                }

                if (c.HasChildren) ApplyControlColors(c);
            }
        }

        private void RenderSpaceList()
        {
            _cardsFlowPanel.Controls.Clear();

            var filtered = _spaces.Where(s =>
                (_selectedCategoryFilter == "All" || s.Category == _selectedCategoryFilter) &&
                (string.IsNullOrEmpty(_txtSearch.Text) || s.Title.Contains(_txtSearch.Text) || s.Description.Contains(_txtSearch.Text)) &&
                (_cboCapacity.SelectedIndex == 0 ||
                 (_cboCapacity.SelectedIndex == 1 && s.Capacity == 1) ||
                 (_cboCapacity.SelectedIndex == 2 && s.Capacity <= 4) ||
                 (_cboCapacity.SelectedIndex == 3 && s.Capacity >= 5))
            ).ToList();

            foreach (var space in filtered)
            {
                Panel card = CreateSpaceCard(space);
                _cardsFlowPanel.Controls.Add(card);
            }

            _lblTotalSpace.Text = $"{_spaces.Count} 개";
            _lblWishlist.Text = $"{_spaces.Count(s => s.IsLiked)} 개";
            _lblWishlist.ForeColor = Color.FromArgb(225, 29, 72);
        }

        private Panel CreateSpaceCard(SpaceModel space)
        {
            Panel card = new Panel
            {
                Size = new Size(250, 200),
                BackColor = _bgCard,
                Margin = new Padding(6),
                Padding = new Padding(10),
                BorderStyle = BorderStyle.FixedSingle,
                Cursor = Cursors.Hand
            };

            card.Click += (s, e) => SelectSpace(space);

            Label lblCategory = new Label { Text = space.CategoryName, ForeColor = Color.FromArgb(14, 165, 233), Font = new Font("맑은 고딕", 8F, FontStyle.Bold), Location = new Point(10, 8), AutoSize = true };
            Label lblTitle = new Label { Text = space.Title, ForeColor = _textMain, Font = new Font("맑은 고딕", 9.5F, FontStyle.Bold), Location = new Point(10, 26), Size = new Size(200, 38) };
            Label lblDesc = new Label { Text = space.Description, ForeColor = _textSub, Font = new Font("맑은 고딕", 8F), Location = new Point(10, 65), Size = new Size(225, 36) };
            Label lblAmenities = new Label { Text = space.Amenities, ForeColor = Color.FromArgb(2, 132, 199), Font = new Font("맑은 고딕", 7.5F, FontStyle.Bold), Location = new Point(10, 108), AutoSize = true };
            Label lblPrice = new Label { Text = $"{space.PricePerHour:N0}원 / 시간", ForeColor = Color.FromArgb(37, 99, 235), Font = new Font("맑은 고딕", 9.5F, FontStyle.Bold), Location = new Point(10, 142), AutoSize = true };
            Label lblCapacity = new Label { Text = $"최대 {space.Capacity}인", ForeColor = _textSub, Font = new Font("맑은 고딕", 8F), Location = new Point(175, 145), AutoSize = true };

            Button btnLike = new Button
            {
                Text = space.IsLiked ? "♥" : "♡",
                Size = new Size(25, 25),
                Location = new Point(215, 5),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.FromArgb(225, 29, 72),
                Font = new Font("맑은 고딕", 10, FontStyle.Bold)
            };
            btnLike.FlatAppearance.BorderSize = 0;
            btnLike.Click += (s, e) =>
            {
                space.IsLiked = !space.IsLiked;
                btnLike.Text = space.IsLiked ? "♥" : "♡";
                _lblWishlist.Text = $"{_spaces.Count(sp => sp.IsLiked)} 개";
            };

            foreach (Control c in new Control[] { lblCategory, lblTitle, lblDesc, lblAmenities, lblPrice, lblCapacity })
            {
                c.Click += (s, e) => SelectSpace(space);
                card.Controls.Add(c);
            }
            card.Controls.Add(btnLike);

            return card;
        }

        private void SelectSpace(SpaceModel space)
        {
            _selectedSpace = space;
            _selectedHours.Clear();

            _lblSelectedTitle.Text = space.Title;
            _lblSelectedPrice.Text = $"{space.PricePerHour:N0}원 / 시간";

            RenderTimeChips();
            RenderReviews();
            UpdateTotalPrice();
        }

        private void RenderTimeChips()
        {
            _timeChipsPanel.Controls.Clear();

            // [수정] 공간이 선택되지 않았을 때는 안 뜨는 게 아니라 안내 라벨 출력
            if (_selectedSpace == null)
            {
                Label lblNotice = new Label
                {
                    Text = "👈 왼쪽에서 원하시는 공간을\n   클릭하면 이용 시간대가 표시됩니다.",
                    Font = new Font("맑은 고딕", 9F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(100, 116, 139),
                    AutoSize = false,
                    Size = new Size(280, 80),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                _timeChipsPanel.Controls.Add(lblNotice);
                return;
            }

            // 공간이 선택되었을 때 09:00~20:00 시간대 버튼 생성
            for (int hour = 9; hour <= 20; hour++)
            {
                int h = hour;
                bool isReserved = _selectedSpace.ReservedHours.Contains(h);
                bool isSelected = _selectedHours.Contains(h);

                Button chip = new Button
                {
                    Text = $"{h:D2}:00",
                    Size = new Size(58, 28),
                    Margin = new Padding(2),
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("맑은 고딕", 8F, FontStyle.Bold),
                    Enabled = !isReserved,
                    Cursor = isReserved ? Cursors.Default : Cursors.Hand
                };

                if (isReserved)
                {
                    chip.BackColor = Color.FromArgb(203, 213, 225);
                    chip.ForeColor = Color.Gray;
                }
                else if (isSelected)
                {
                    chip.BackColor = Color.FromArgb(37, 99, 235);
                    chip.ForeColor = Color.White;
                }
                else
                {
                    chip.BackColor = _bgInput;
                    chip.ForeColor = _textMain;
                }

                chip.Click += (s, e) =>
                {
                    if (_selectedHours.Contains(h)) _selectedHours.Remove(h);
                    else _selectedHours.Add(h);

                    RenderTimeChips();
                    UpdateTotalPrice();
                };

                _timeChipsPanel.Controls.Add(chip);
            }
        }

        private void UpdateTotalPrice()
        {
            if (_selectedSpace == null) return;
            int total = _selectedHours.Count * _selectedSpace.PricePerHour;
            _lblTotalPrice.Text = $"{total:N0}원";
        }

        private void RenderReviews()
        {
            _reviewsFlowPanel.Controls.Clear();
            if (_selectedSpace == null) return;

            foreach (var r in _selectedSpace.Reviews)
            {
                Panel p = new Panel { Size = new Size(290, 40), BackColor = _bgInput, Margin = new Padding(2), Padding = new Padding(4) };
                Label lblAuthor = new Label { Text = $"{r.Author} ({new string('★', r.Rating)})", Font = new Font("맑은 고딕", 7.5F, FontStyle.Bold), ForeColor = _textMain, Dock = DockStyle.Top };
                Label lblComment = new Label { Text = r.Comment, Font = new Font("맑은 고딕", 7.5F), ForeColor = _textSub, Dock = DockStyle.Fill };

                p.Controls.Add(lblComment);
                p.Controls.Add(lblAuthor);
                _reviewsFlowPanel.Controls.Add(p);
            }
        }

        private void BtnSaveReview_Click(object sender, EventArgs e)
        {
            if (_selectedSpace == null || string.IsNullOrWhiteSpace(_txtReviewComment.Text)) return;

            int rating = 5 - _cboRating.SelectedIndex;
            _selectedSpace.Reviews.Add(new ReviewModel { Author = _txtUserName.Text, Rating = rating, Comment = _txtReviewComment.Text });
            _txtReviewComment.Clear();
            RenderReviews();
        }

        private void BtnPay_Click(object sender, EventArgs e)
        {
            if (_selectedSpace == null)
            {
                MessageBox.Show("대여할 공간을 먼저 선택해주세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_selectedHours.Count == 0)
            {
                MessageBox.Show("이용하실 시간대를 최소 1개 이상 선택해주세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _selectedHours.Sort();
            _lblModalTitle.Text = _selectedSpace.Title;
            _lblModalTime.Text = $"{_datePicker.Value:yyyy-MM-dd} ({string.Join(", ", _selectedHours)}시)";
            _lblModalPrice.Text = $"최종 금액: {_lblTotalPrice.Text}";

            _paymentModalOverlay.Visible = true;
            _paymentModalOverlay.BringToFront();
        }

        private void BtnConfirmPayment_Click(object sender, EventArgs e)
        {
            string payMethod = _radioKakao.Checked ? "카카오페이" : (_radioToss.Checked ? "토스페이" : "신용카드");

            ReservationModel res = new ReservationModel
            {
                UserName = _txtUserName.Text,
                UserPhone = _txtUserPhone.Text,
                SpaceTitle = _selectedSpace.Title,
                DateText = _datePicker.Value.ToString("yyyy-MM-dd"),
                TimeText = $"{string.Join(",", _selectedHours)}시",
                PaymentMethod = payMethod,
                PriceText = _lblTotalPrice.Text,
                SpaceId = _selectedSpace.Id,
                Hours = new List<int>(_selectedHours)
            };

            _reservations.Add(res);
            _selectedSpace.ReservedHours.AddRange(_selectedHours);

            MessageBox.Show("결제 및 공간 예약이 성공적으로 완료되었습니다!", "예약 성공", MessageBoxButtons.OK, MessageBoxIcon.Information);

            _paymentModalOverlay.Visible = false;
            _selectedHours.Clear();

            _lblReservationCount.Text = $"{_reservations.Count} 건";

            RenderTimeChips();
            UpdateTotalPrice();
            RefreshHistoryGrid();
        }

        private void RefreshHistoryGrid()
        {
            _gridHistory.Rows.Clear();
            foreach (var r in _reservations)
            {
                _gridHistory.Rows.Add(r.UserName, r.UserPhone, r.SpaceTitle, r.DateText, r.TimeText, r.PaymentMethod, r.PriceText);
            }
        }

        private void GridHistory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 7)
            {
                var res = _reservations[e.RowIndex];
                var space = _spaces.FirstOrDefault(s => s.Id == res.SpaceId);

                if (space != null)
                {
                    foreach (var h in res.Hours) space.ReservedHours.Remove(h);
                }

                _reservations.RemoveAt(e.RowIndex);
                _lblReservationCount.Text = $"{_reservations.Count} 건";

                RefreshHistoryGrid();
                RenderTimeChips();
                MessageBox.Show("예약이 취소되었습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void SwitchNav(bool showReserve)
        {
            _viewReserve.Visible = showReserve;
            _viewHistory.Visible = !showReserve;
            if (!showReserve) RefreshHistoryGrid();
        }

        private void CategoryFilter_Click(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                _selectedCategoryFilter = btn.Tag.ToString();
                RenderSpaceList();
            }
        }
    }
}