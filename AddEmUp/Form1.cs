using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;

namespace AddEmUp
{
    /*
     * Main application class 
     */
    public partial class Form1 : Form
    {
        /*
         * Member variable: array of ints
         * Comments: The dimensions of buttons to be displayed (level dependent):
         *           'grid_dimension' will be one of these values and will be the indexed value squared
         *           Array bound checked in implementation to never be greater than 6, although max level number is
         *           undetermined.
         */
        int[] cell_dim_vector = new int[7] { 5, 7, 8, 10, 14, 16, 20 };
        /*
         * Member variable: array of ints
         * Comments: Size of buttons to be displayed (level dependent):
         *           'button_size' will be one of these values and will be the indexed value squared.
         *           Array bound checked in implementation to never be greater than 6, although max level number is
         *           undetermined.
         */
        int[] cell_buttonsize_vector = new int[7] { 112, 80, 70, 56, 40, 35, 28 };
        /*
         * Member variable: array of ints
         * Comments: Index represents (level_index - 1)
         *           The value represents the index of cell_dim_vector or cell_buttonsize_vector to be accessed.
         *           If player is on level 4 for instance, button_size will be set to ->
         *           cell_buttonsize_vector[dimsize_level_vector_indices[3]] (which is 70 pixels).
         *           cell_dim_vector is accessed in a similar manner.
         *           Array bound checked in implementation to never be greater than 17, although max level number is
         *           undetermined.
         */
        int[] dimsize_level_vector_indices = new int[18] {0,1,1,2,2,2,3,3,3,4,4,4,4,5,5,5,5,6};
        /*
         * Member variable: array of ints
         * Comments: Index represents (level_index - 1)
         *           Each int represents the start timer value.
         *           If player is on level 7 for instance, the start timer will be set to 17.
         *           Array bound checked in implementation to never be greater than 17, although max level number is
         *           undetermined.
         */
        int[] start_timer_values_vector = new int[18] {15,16,16,17,17,17,18,18,18,19,19,19,19,20,20,20,20,21};
        /* 
         * Member variable: int
         * Comments: This is the number of cells along a row or column for a given level.
         *           Number of cells existing at any given time is 'grid_dimension' * 'grid_dimension'
         */
        int grid_dimension;
        /* 
         * Member variable: int
         * Comments: This is the width and height of every cell on the playing field.
         *           'grid_dimension' * 'button_size' will always equal 560, to occupy the same
         *           amount of space on the main form.
         */
        int button_size;     
        /*
         * Member variable: const int
         * Comments: This is the size of the menu toolbar at the top of the screen in height (pixels).
         *           The playing field of buttons (or cells) is offset from the top by this value.
         */
        const int menu_strip_height = 24;
        /*
         * Member variable: const int
         * Comments: A button may have one of 'num_colors' different colors
         */
        const int num_colors = 6;
        /*
         * Member Variable: List<cell>
         * Comments: A cell is a struct consisting of 'Controls.Button' object as well as various data
         *           fields about the cell. 'cells.Count' will always be equal to either 'grid_dimension' * 'grid_dimension' or 0.
         */
        List<cell> cells = new List<cell>();
        /*
         * Member Variable: Array of Colors
         * Comments: An array of all the possible colors a cell's button object may be.
         *           Values are tweaked RGB configurations of red, yellow, green, blue, orange, and purple.
         */
        Color[] cell_colors = new Color[num_colors] { Color.FromArgb(255,48,48),
                                                      Color.FromArgb(212,212,0),
                                                      Color.FromArgb(32,255,32),
                                                      Color.FromArgb(64,64,255),
                                                      Color.FromArgb(255,128,0),
                                                      Color.FromArgb(196,0,196) }; 
        /*
         * Member Variable: Random (System random number generator)
         * Comments: Used to generate pseudo random numbers such as button color indices and button values.
         */
        Random rng = new Random();

        /*
         * Member Variable: int
         * Comments: This is current level (number) the player is on.
         */
        int level_index;
        /*
         * Member Variable: int
         * Comments: The goal sum the player must reach.
         */
        int goal_value;
        /*
         * Member Variable: int
         * Comments: The time the player has left to reach the goal value
         *           before the game is lost or the next level is reached.
         */
        int timer_val;
        /*
         * Member Variable: int
         * Comments: The total sum of all selected (highlighted) cells.
         */
        int total_selected_val;
        /*
         * Member Variable: int
         * Comments: The player's total score over the duration of a current game.
         */
        int total_score;
        /*
         * Member Variable: int
         * Comments: The number of cells currently selected (highlighted)
         */
        int num_selected;
        /*
         * Member Variable: int
         * Comments: The number of cells cleared over the duration of the level.
         */
        int num_cleared;
        /*
         * Member Variable: int
         * Comments: May be either 1 or 10.
         *           10 goal value reached and all selected cells had the same color,
         *           1 otherwise.
         */
        int bonus_factor;
        /*
         * Member Variable: float
         * Comments: Ratio of number of cleared cells over total number of cells
         *           present at the start of the level, multiplied by 100.0f.
         */
        float percent_cleared = 0.0f;
        /*
         * Member Variable: Is the game currently active (i.e. timer running)
         * Comments: Is false right at the beginning of a game, after the completion of a level or after the game is lost.
         *           The timer is running only when true.
         */
        bool game_active = false;
        /*
         * Member Variable: Array of Ints
         * Comments: Number of Red, Yellow, Green, Blue, Orange and Purple 
         *           colors currently selected. Used for determining value
         *           of 'bonus_factor'.
         */ 
        int[] color_indices_selected = new int[num_colors] { 0, 0, 0, 0, 0, 0 };
        /*
         * Member Variable: Label (control object)
         * Comments: This is the control object that displays the value of 'goal_value'
         */
        Label goal_label = new Label();
        /*
         * Member Variable: Timer (control object)
         * Comments: This is the control object that represents the timer.
         */
        Timer goal_timer = new Timer();
        /*
         * Member Variable: Label (control object)
         * Comments: This is the control object that displays the value of 'timer_val'.
         */
        Label timer_label = new Label();
        /*
         * Member Variable: Label (control object)
         * Comments: This is the control object that displays the value of 'total_score'
         */
        Label score_label = new Label();
        /*
         * Member Variable: Label (control object)
         * Comments: This is the control object that displays the value of 'percent_cleared'
         */
        Label percent_label = new Label();
        /*
         * Member Variable: Label (control object)
         * Comments: This is the control object that displays the value of 'level_index'
         */
        Label level_label = new Label();
        /*
         * Member Variable: Button (control object)
         * Comments: This is the control object that represents the start level ('Begin')
         *           button. Pressing this button indirectly starts the timer and starts the level.
         */
        Button start_game_button = new Button();
        /*
         * Member Variable: Button (control object)
         * Comments: This is the control object that represents the next level ('Next Level')
         *           button. Pressing this button indirectly stops the timer and proceeds to the next
         *           level, there the 'start_game_button' must be pressed to begin the round and
         *           restart the timer.
         */
        Button goto_next_level_button = new Button();
        /*
         * Member Variable: List of 'highScore' objects
         * Comments: Represents the serialized list of high scores to be stored and retrieved
         *           from an XML file, and viewed via the menu toolstrip at the top of the form.
         */
        public List<highScore> high_scores = new List<highScore>();
        /*
         * Member Function: Constructor
         * Comments: Calls 'InitializeComponent()', which initializes the values associated with the 
         *           designer configuration in the designer file.
         */
        public Form1()
        {
            InitializeComponent();
        }
        /*
         * Member Function: void
         * Comments: Configures everything that wasn't initialized by 'InitializeComponent()',
         *           as well as initializes various game parameters. Called right after the constructor.
         */
        private void Form1_Load(object sender, EventArgs e)
        {
            deSerializeHighScores();
            level_index = 1;
            grid_dimension = cell_dim_vector[0];
            button_size = cell_buttonsize_vector[0];
            resetLevelVariables();
            addLevelButtons();
            addLevelGoalLabel();
            addTotalScoreLabel();
            addPercentLabel();
            addLevelLabel();
            configureTimerLabel();
            configureTimer();
            configureStartButton();
            configureNextLevelButton();
        }
        /*
         * Member Function: private void
         * Comments: Sets up all cells, pushes them to the cell list, and adds their 
         *           corresponding buttons to the form controls.
         */
        private void addLevelButtons()
        {
            // local variables used to set each cell value and color.
            int value, color_index;
            for (int y = 0; y < grid_dimension; y++)
            {
                for (int x = 0; x < grid_dimension; x++)
                {
                    // Each cell value to be clicked on and added up to 'goal_value' must 
                    // be between 1 and 9 inclusive.
                    value = rng.Next(1, 10);
                    // The index used to retrieve the cell color.
                    color_index = rng.Next(0, num_colors);
                    Button newCellButton = new Button();
                    newCellButton.Size = new Size(button_size, button_size);
                    newCellButton.Top = (y * button_size) + menu_strip_height;
                    newCellButton.Left = x * button_size;
                    // 'button_size/2 - 1' chosen the fit number font size onto button in
                    // a consistent and readable fashion depending which one of the seven
                    // button sizes in cell_buttonsize_vector button_size is set to.
                    newCellButton.Font = new Font("Arial", button_size/2 - 1, FontStyle.Bold);
                    newCellButton.Click += new EventHandler(cellClick);
                    setVariableButtonDisplayParameters(newCellButton,value,color_index);
                    // Create a cell with this button, push it onto the cell list and add it to
                    // the controls.
                    cell newCell = new cell(newCellButton, value, color_index);
                    this.Controls.Add(newCellButton);
                    cells.Add(newCell);
                }
            }
        }
        /*
         * Member Function: private void
         * Comments: Configures and adds the 'goal_label' Label to the controls.
         *           Is hidden (shown as '<>') if timer is stopped and game is not active.
         */
        private void addLevelGoalLabel()
        {
            goal_label.Top = 59;
            goal_label.Left = 632;
            goal_label.Size = new Size(86, 56);
            goal_label.BackColor = Color.Black;
            goal_label.Text = "<>";
            goal_label.TextAlign = ContentAlignment.MiddleCenter;
            goal_label.Font = new Font("Courier", 36, FontStyle.Bold);
            this.Controls.Add(goal_label);
        }
        /*
         * Member Function: private void
         * Comments: Configures and adds the 'start_game_button' Button to the controls.
         */
        private void configureStartButton()
        {
            start_game_button.Top = 474;
            start_game_button.Left = 572;
            start_game_button.Size = new Size(200, 96);
            start_game_button.BackColor = Color.DarkGray;
            start_game_button.ForeColor = Color.Black;
            start_game_button.Text = "Begin";
            start_game_button.Font = new Font("Serif", 16, FontStyle.Bold);
            start_game_button.Click += new EventHandler(startGameButtonClick);
            this.Controls.Add(start_game_button);
        }
        /*
         * Member Function: private void
         * Comments: Configures and adds the 'goto_next_level_button' Button to the controls.
         */
        private void configureNextLevelButton()
        {
            goto_next_level_button.Top = 256;
            goto_next_level_button.Left = 624;
            goto_next_level_button.Size = new Size(100, 24);
            goto_next_level_button.BackColor = Color.LightGray;
            goto_next_level_button.ForeColor = Color.DarkGreen;
            goto_next_level_button.Text = "Next Level";
            goto_next_level_button.Font = new Font("Serif", 10, FontStyle.Bold);
            goto_next_level_button.Hide();
            goto_next_level_button.Click += new EventHandler(gotoNextLevelButtonClick);
            this.Controls.Add(goto_next_level_button);
        }
        /*
         * Member Function: private void
         * Comments: Configures and adds the 'score_label' Label to the controls.
         */
        private void addTotalScoreLabel()
        {
            score_label.Top = 164;
            score_label.Left = 632;
            score_label.Size = new Size(86, 56);
            score_label.BackColor = Color.Black;
            score_label.Text = total_score.ToString();
            score_label.TextAlign = ContentAlignment.MiddleCenter;
            score_label.Font = new Font("Courier", 12, FontStyle.Bold);
            this.Controls.Add(score_label);
        }
        /*
         * Member Function: private void
         * Comments: Configures and adds the 'timer_label' Label to the controls.
         */
        private void configureTimerLabel()
        {
            timer_label.Top = 324;
            timer_label.Left = 572;
            timer_label.Size = new Size(200, 96);
            timer_label.BackColor = Color.Black;
            setTimerLabelTextAndColor();
            timer_label.TextAlign = ContentAlignment.MiddleCenter;
            timer_label.Font = new Font("Serif", 72, FontStyle.Bold);
            this.Controls.Add(timer_label);
        }
        /*
         * Member Function: private void
         * Comments: Configures the goal_timer object. The timer will tick every 1000 milliseconds,
         *           timerTick will be called once every tick.
         */
        private void configureTimer()
        {
            goal_timer.Interval = 1000;
            goal_timer.Tick += new EventHandler(timerTick);
        }
        /*
         * Member Function: private void
         * Comments: Configures and adds the 'percent_label' Label to the controls.
         */
        private void addPercentLabel()
        {
            percent_label.Top = 442;
            percent_label.Left = 572;
            percent_label.Size = new Size(240, 25);
            percent_label.ForeColor = Color.Black;
            setPercentLabelTextAndColor();
            percent_label.Font = new Font("Arial", 12, FontStyle.Bold);
            this.Controls.Add(percent_label);
        }
        /*
         * Member Function: private void
         * Comments: Configures and adds the 'level_label' Label to the controls.
         */
        private void addLevelLabel()
        {
            level_label.Top = 224;
            level_label.Left = 625;
            level_label.Size = new Size(100,25);
            level_label.ForeColor = Color.Black;
            level_label.TextAlign = ContentAlignment.MiddleCenter;
            level_label.Font = new Font("Serif", 12, FontStyle.Bold);
            level_label.Text = "Level: " + level_index.ToString();
            this.Controls.Add(level_label);
        }
        /*
         * Member Function: void
         * Comments: Helper that sets a button object's value and color.
         */
        void setVariableButtonDisplayParameters(Button b, int val, int col_idx)
        {
            b.BackColor = cell_colors[col_idx];
            // Foreground color is white when button's cell parent 'ctive' field is false.
            // Otherwise, foreground and background are reversed. 
            b.ForeColor = Color.White;
            b.Text = val.ToString();
        }
        /*
         * Member Function: void
         * Comments: Helper that sets the 'timer_label' Text field to show the 'timer_val'
         *           value. The color indicates when the timer is about to run out (yellow -> running low,
         *           red -> running very low).
         */
        void setTimerLabelTextAndColor()
        {
            timer_label.Text = timer_val.ToString();
            if (timer_val <= 5)
                timer_label.ForeColor = cell_colors[0];
            else if (timer_val <= 10)
                timer_label.ForeColor = cell_colors[1];
            else
                timer_label.ForeColor = cell_colors[2];
        }
        /*
         * Member Function: void
         * Comments: Event handler that is triggered when the 'start_game_button' Button
         *           is clicked. Does nothing if game is already active; otherwise, the timer is 
         *           started and game is set to active.
         */
        void startGameButtonClick(object sender, EventArgs e)
        {
            if (game_active)
                return;
            // (see comments below)
            generateNewGoalValue();
            game_active = true;
            goal_timer.Start();
        }
        /*
         * Member Function: void
         * Comments: Event handler called when 'goto_next_level_button' is shown and clicked.
         *           Calls the function that sets up the next level.
         */
        void gotoNextLevelButtonClick(object sender, EventArgs e)
        {
            // (may be unnecessary)
            if (!game_active)
                return;
            // Adds level bonus to score and goes to next level.
            setupLevelControlsAndParameters(level_index+1,total_score + (num_cleared * 50));
        }
        /*
         * Member Function: void
         * Comments: Helper that sets the goal_value to a random number, set the timer value
         *           to a value in 'start_timer_values_vector' (depending on the level). The Lavel 
         *           corresponding to each and updated as well.
         */
        void generateNewGoalValue()
        {
            goal_value = rng.Next(1, getMaxGoalValue());
            goal_label.Text = goal_value.ToString();
            timer_val = getStartTimerValue();
            setTimerLabelTextAndColor();
        }
        /*
         * Member Function: void
         * Comments: Event handler for when a cell's button is clicked.
         */
        void cellClick(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            // Find the cell whose Button object is the Button object from the controls
            // that was clicked...
            foreach (cell c in cells)
            {
                if (b == c.btn)
                {
                    // ...and activate that cell and check game parameter's resulting from
                    // this action.
                    registerCellClickData(b,c);
                    return;
                }
            }
        }
        /*
         * Member Function: void
         * Comments: Helper called by cellClick.
         * Parameters: Button in the form controls and cell in the list of cells containing that button.
         */
        void registerCellClickData(Button control_button, cell cell_obj)
        {
            // Do nothing if 'start_game_button' was not pressed.
            if (!game_active)
                return;
            // Swap foreground and background colors of button in the controls (highlight) to indicate
            // that the cell is active.
            Color temp = control_button.ForeColor;
            control_button.ForeColor = control_button.BackColor;
            control_button.BackColor = temp;
            cell_obj.setActive(!cell_obj.active);

            // If cell was activated, add cell's value to 'total_selected_value' and update the 
            // array indicating number of each of the 6 colors that are active. Increment field 
            // corresponding to number of active cells.
            if (cell_obj.active)
            {
                total_selected_val += cell_obj.value;
                color_indices_selected[cell_obj.color_index]++;
                num_selected++;
            }
            // If cell was deactivated, subtract cell's value from 'total_selected_value' and update the 
            // array indicating number of each of the 6 colors that are active. Decrement field
            // corresponding to number of active cells.
            else
            {
                total_selected_val -= cell_obj.value;
                color_indices_selected[cell_obj.color_index]--;
                num_selected--;
            }

            // If all selected cells are of the same color and 4 or more cells are active,
            // set the 'bonus_factor' field to 10. In this case, 5 of the 6 'color_indices_selected'
            // array values are 0, and one of them equals 'num_selected'.
            bonus_factor = 1;
            for (int i = 0; i < num_colors; ++i)
            {
                if (color_indices_selected[i] == num_selected && num_selected > 3)
                    bonus_factor = 10;
            }
            // Execute code that should be run when the goal value was reached before the timer 
            // hits zero.
            if (total_selected_val == goal_value)
            {
                // increment 'total_score' and it's label.
                total_score += (goal_value * num_selected * bonus_factor);
                score_label.Text = total_score.ToString();
                // Deactivate and HIDE the cells that were selected when goal value was reached,
                // helping indicate which cells have been cleared up to this point.
                clearAllSelectedCells();
                generateNewGoalValue();
                // set 'percent_cleared' to appropriate value and configure its display Label.
                percent_cleared = ((float)num_cleared * 100.0f) / (float)(grid_dimension * grid_dimension);
                setPercentLabelTextAndColor();
                // Make visible the next level button if 50% or more of the level has been cleared,
                // so that the user might not have to wait for the timer to reach zero to go to the
                // next level under the correct circumstance.
                if (percent_cleared >= 50.0f)
                {
                    goto_next_level_button.Show();
                }
                // Determine if all cells have been cleared on the level.
                checkPossibleLevelChangeClickHelper();
            }
        }
        /*
         * Member Function: void
         * Comments: Event handler called every second. Decrements time remaining (and updates Label),
         *           Also see if timer has reached zero.
         */
        void timerTick(object sender, EventArgs e)
        {
            timer_val--;
            setTimerLabelTextAndColor();
            checkPossibleLevelChangeTimeoutHelper();
        }
        /*
         * Member Function: void
         * Comments: Helper called by timerTick.
         *           If timer is not zero, do nothing. Otherwise, check if player has cleared
         *           enough to advance to next level (with end level bonus); if not, the game
         *           is over and the final scores are updated.
         */
        void checkPossibleLevelChangeTimeoutHelper()
        {
            if (timer_val == 0)
            {
                if (percent_cleared >= 50.0f)
                {
                    // Adds level bonus to score and goes to next level.
                    setupLevelControlsAndParameters(level_index + 1, total_score + (num_cleared * 50));
                }
                // In this else statement code block, all end game code is executed.
                else
                {
                    // Halt timer; indicate the game is over and show final score (in a message box).
                    goal_timer.Stop();
                    MessageBox.Show("Game Over! Final Score: " + total_score.ToString());
                    // Add current score to 'high_scores' list (and subsequently show those scores in
                    // a message box).
                    addToHighScores();
                    // Start back to level 1 and reset score to 0.
                    setupLevelControlsAndParameters(1,0);
                }
            }
        }
        /*
         * Member Function: void
         * Comments: Helper called by registerCellClickData().
         *           Does nothing if not all cells are cleared; otherwise immediately go to the next
         *           level and add level bonus and special level bonus (when all cells are cleared).
         */
        void checkPossibleLevelChangeClickHelper()
        {
            if (num_cleared == grid_dimension * grid_dimension)
            {
                setupLevelControlsAndParameters(level_index + 1, total_score + (level_index * num_cleared * 50) + (level_index * 10000));
            }
        }
        /*
         * Member Function: void
         * Comments: Does everything to set up a level.
         * Parameters: New level value, new score value (updated from bonuses or reset to 0).
         */
        void setupLevelControlsAndParameters(int new_lev_idx, int new_score)
        {
            // update fields from parameters indicating new level and new score.
            level_index = new_lev_idx;
            total_score = new_score;
            goal_timer.Stop();
            goto_next_level_button.Hide();
            // Clear all cells from cell list.
            cells.Clear();
            // Dispose of all playing field Buttons from the controls.
            for (int i = Controls.Count - 1; i >= 0; i--)
            {
                if (Controls[i] is Button && Controls[i] != start_game_button && Controls[i] != goto_next_level_button)
                {
                    Controls[i].Click -= new EventHandler(cellClick);
                    Controls[i].Dispose();
                }
            }
            // set 'grid_dimension' and 'button_size' based on incoming level parameter.
            int index = level_index <= 18 ? dimsize_level_vector_indices[level_index - 1] : 6;
            grid_dimension = cell_dim_vector[index];
            button_size = cell_buttonsize_vector[index];
            // Refill cell list and add back corresponding playing field buttons to controls.
            // Calling this function here sets up the cells and buttons for a new level.
            addLevelButtons();
            resetLevelVariables();
            game_active = false;
            goal_label.Text = "<>";
            score_label.Text = total_score.ToString();
            level_label.Text = "Level: " + level_index.ToString();
            setPercentLabelTextAndColor();
            setTimerLabelTextAndColor();
        }
        /*
         * Member Function: void
         * Comments: Sets 'percent_label' label to display the value of 'percent_cleared'.
         *           Is set to green when 50% or more of the level was cleared indicating the
         *           next level is guaranteed.
         */
        void setPercentLabelTextAndColor()
        {
            percent_label.Text = percent_cleared.ToString() + " percent cleared";
            if (percent_cleared >= 50.0f)
                percent_label.ForeColor = Color.Green;
            else
                percent_label.ForeColor = Color.Black;
        }
        /*
         * Member Function: void
         * Comments: Sets up starting values for various fields representing level state.
         */
        void resetLevelVariables()
        {
            goal_value = rng.Next(1, getMaxGoalValue());
            total_selected_val = 0;
            timer_val = getStartTimerValue();
            num_selected = 0;
            num_cleared = 0;
            percent_cleared = 0.0f;
            bonus_factor = 1;
            for (int i = 0; i < num_colors; i++)
            {
                color_indices_selected[i] = 0;
            }
        }
        /*
         * Member Function: void
         * Comments: When a goal value is reached, all highlighted cells are hidden and deactivated
         *           to indicate that they have been cleared.
         */
        void clearAllSelectedCells()
        {
            for (int i = 0; i < grid_dimension*grid_dimension; i++)
            {
                if (cells[i].active)
                {
                    cells[i].setActive(false);
                    cells[i].btn.Hide();
                }
            }
            
            // Some level parameters are also updated when a goal value was reached.
            // [This probably should be made into its own function] 
            for (int i = 0; i < num_colors; i++)
            {
                color_indices_selected[i] = 0;
            }
            num_cleared += num_selected;
            total_selected_val = 0;
            num_selected = 0;
        }
        /*
         * Member Function: void
         * Comments: Does nothing if high scores XML file does not exist; otherwise shove all
         *           fields in the XML file to the 'high_scores' list object.
         */
        void deSerializeHighScores()
        {
            // Necessary to avoid exception. Only returns at this point up until the first 
            // high score is created. (Checks if present in executable directory)
            if (!File.Exists("addemupscores.xml"))
                return;

            var serializer = new XmlSerializer(high_scores.GetType(), "addemupscores.scores");
            object obj;
            using (var reader = new StreamReader("addemupscores.xml"))
            {
                obj = serializer.Deserialize(reader.BaseStream);
            }
            high_scores = (List<highScore>)obj;
            // Order and truncate 'high_scores' list.
            parseHighScores();
        }
        /*
         * Member Function: void
         * Comments: Add player's score to high_scores list and serialize it with XML file.
         *           When the application exits, updated high scores will be in the XML file to
         *           be reloaded into memory when the application begins. If high score XML file
         *           does not exist, it is created with the sole field with value of 'total_score'.
         */
        void addToHighScores()
        {
            if (nameFieldMenuItem.Text == "")
                nameFieldMenuItem.Text = "Player";
            var score = new highScore() { score = total_score, level_idx = level_index, name = nameFieldMenuItem.Text };
            high_scores.Add(score);
            var serializer = new XmlSerializer(high_scores.GetType(), "addemupscores.scores");
            using (var writer = new StreamWriter("addemupscores.xml", false))
            {
                serializer.Serialize(writer.BaseStream, high_scores);
            }
            // Additionally:
            // Order and truncate 'high_scores' list.
            parseHighScores();
            // Show all high scores in a message box.
            showHighScoresMBox();
        }
        /*
         * Member Function: void
         * Comments: Helper that sorts 'high_scores' from highest to lowest score. Also, we only
         *           want to save the top 15 high scores.
         */
        void parseHighScores()
        {
            high_scores.Sort((a, b) => a.score.CompareTo(b.score));
            high_scores.Reverse();
            if (high_scores.Count() > 15)
                high_scores = high_scores.GetRange(0, 15);
        }
        /*
         * Member Function: void
         * Comments: Parse through the 'high_scores' list and show all the elements in a message box.
         *           Each element contains a score (list is ordered by this), a level number, and 
         *           the player's name. Each element is numbered from 1 to min(sizeof(list),15).
         *           Can be called when clicking 'highScoresMenuItem' item in toolstrip at the
         *           top of the form.
         */
        void showHighScoresMBox()
        {
            String s = "";
            int max_val = high_scores.Count <= 15 ? high_scores.Count : 15;
            if (max_val == 0)
                s = "None to display.";
            else
            {
                for (int i = 0; i < max_val; ++i)
                {
                    s += (i+1).ToString() + ") " + high_scores[i].name + ": " + high_scores[i].score.ToString() + " (level " + high_scores[i].level_idx.ToString() + ")\n";
                }
            }
            MessageBox.Show(s, "High Scores");
        }
        /*
         * Member Function: void
         * Comments: Can be called when clicking 'aboutMenuItem' item in toolstrip at the
         *           top of the form.
         */
        void showAboutMBox()
        {
            MessageBox.Show("Add Em Up! \nA WFA application written in C# \n\u00a9 Eric Wolfson 2016","About");
        }
        /*
         * Member Function: void
         * Comments: Can be called when clicking 'instructionsMenuItem' item in toolstrip at the
         *           top of the form.
         */
        void showInstructionsMBox()
        {
            String s = "";
            s += "-) Click number tiles such that the sum equals the goal value!\n";
            s += "-) You must clear at least 50 percent of the level to advance.\n";
            s += "-) If you fail to do so when the timer reaches 0, the game is over.\n";
            s += "-) The number of tiles to clear increases with the level number.\n";
            s += "-) The formula for scoring when goal value is matched is:\n";
            s += "    NUM_SELECTED x GOAL_SCORE x COLOR_BONUS\n";
            s += "-) The color bonus is 10 if all colors match when 4 or more are selected;\n";
            s += "    otherwise, the color bonus is 1.\n";
            s += "-) After clearing a level, you get a bonus of 50 x NUM_CLEARED x LEVEL_NUMBER.\n";
            s += "-) If all tiles are cleared, another bonus of 10000 x LEVEL_NUMBER is awarded!\n";
            MessageBox.Show(s, "How To Play");
        }
        /*
         * Member Function: void
         * Comments: Get maximum possible 'goal_value' for current level. 'goal_value' is set to
         *           ret_val - 1 and may have a range of 19-99 depending on the random number generator
         *           and the level number. As a levels increases, the maximum goal_value is increases by 4.
         *           'ret_val' is eventually capped at 100 ('goal_value' may never be higher than 99 no
         *           matter what the level number is).
         */
        int getMaxGoalValue()
        {
            int ret_val = 20;
            ret_val += (level_index - 1) * 4;
            if (ret_val > 100)
                ret_val = 100;
            return ret_val;
        }
        /*
         * Member Function: void
         * Comments: 'ret_val' ranges from 15 to 21, depending on the level number. See
         *           'start_timer_values_vector' description above for the pattern for level
         *           numbers less than 18. timer start value is 21 for all level numbers 18 and up.
         */
        int getStartTimerValue()
        {
            int ret_val = level_index <= 18 ? start_timer_values_vector[level_index-1] : 21;
            return ret_val;
        }
        /*
         * Member Function: void
         * Comments: aboutMenuItem control only present in designer file. 
         *           This is the event handler called when the toolstrip item titled "about" is clicked.
         *           Item located under the "Help" button on the toolstrip.
         */
        private void aboutMenuItem_Click(object sender, EventArgs e)
        {
            showAboutMBox();
        }
        /*
         * Member Function: void
         * Comments: instructionsMenuItem control only present in designer file. 
         *           This is the event handler called when the toolstrip item titled "How To Play" is clicked.
         *           Item located under the "Help" button on the toolstrip.
         */
        private void instructionsMenuItem_Click(object sender, EventArgs e)
        {
            showInstructionsMBox();
        }
        /*
         * Member Function: void
         * Comments: highScoresMenuItem control only present in designer file. 
         *           This is the event handler called when the toolstrip item titled "High Scores" is clicked.
         *           Item located under the "Help" button on the toolstrip.
         */
        private void highScoresMenuItem_Click(object sender, EventArgs e)
        {
            showHighScoresMBox();
        }
        /*
         * Member Function: void
         * Comments: newGameMenuItem control only present in designer file. 
         *           This is the event handler called when the toolstrip item titled "New Game" is clicked.
         *           Item located under the "Game" button on the toolstrip.
         */
        private void newGameMenuItem_Click(object sender, EventArgs e)
        {
            setupLevelControlsAndParameters(1, 0);
        }
        /*
         * Member Function: void
         * Comments: exitMenuItem control only present in designer file. 
         *           This is the event handler called when the toolstrip item titled "Exit" is clicked.
         *           Item located under the "Game" button on the toolstrip.
         */
        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }

    /*
     * Cell class
     * Comments: It is necessary to have a list of instantiations of this class rather than
     *           plain Buttons, so that we can associate data with them. A cell is not a Button.
     *           It is a game object.
     */
    public class cell
    {
        /*
         * Member Variable: Button
         * Comments: The form controls have the same instance of this as the list of cells
         *           defined above. Contains all Button data of the cell.
         */
        public Button btn;
        /*
         * Member Variable: Bool
         * Comments: True if cell is selected (and its button highlighted). Otherwise, false.
         *           Set to true when the cell's corresponding button in the form controls has
         *           been clicked.
         */
        public bool active;
        /*
         * Member Variable: int 
         * Comments: Cell's number value.
         */
        public int value;
        /*
         * Member Variable: int 
         * Comments: Cell's color index.
         */
        public int color_index;
        /*
         * Member Function: Constructor
         * Comments: Sets some initial values to the fields. Never called.
         */
        public cell()
        {
            btn = null;
            value = -1;
            color_index = 0;
            active = false;
        }
        /*
         * Member Function: Constructor
         * Comments: All fields initialized with the exception of 'active' being set to false
         *           upon creation.
         */
        public cell(Button b, int val, int col_idx)
        {
            btn = b;
            value = val;
            color_index = col_idx;
            active = false;
        }
        /*
         * Member Function: public void
         * Comments: This sets the cell's 'active' flag.
         */
        public void setActive(bool flg)
        {
            active = flg;
        }
        /*
         * Member Function: public void
         * Comments: This sets the cell's color index value.
         */
        public void setColorIndex(int col_idx)
        {
            color_index = col_idx;
        }
        /*
         * Member Function: public void
         * Comments: This sets the cell's number value.
         */
        public void setValue(int val)
        {
            value = val;
        }
    }
    /*
     * Serializable high score class
     */ 
    [Serializable()]
    public class highScore
    {
        /*
         * Member variable: public int
         * Comments: Set to 'total_score' field in main application class before parent
         * object is added to the 'high_scores' list.
         */
        public int score { get; set; }
        /*
         * Member variable: public int
         * Comments: Set to 'level_index' field in main application class before parent 
         * object is added to the 'high_scores' list.
         */
        public int level_idx { get; set; }
        /*
         * Member variable: public string
         * Comments: Set to the Text field of the 'nameFieldMenuItem' TextBox (located under "Game" button on the toolstrip)
         * configured in designer file, before parent object is added to the 'high_scores' list.
         */
        public string name { get; set; }
    }
}