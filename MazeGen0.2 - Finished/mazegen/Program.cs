using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;


namespace algorithms_practice
{
    class maze_gen
    {
        public int width { get; set; }
        public int height { get; set; }
        public int[,] features;
        public string start;
        public string end;
        public Dictionary<string, string> frontier = new Dictionary<string, string>(); // the string[,] is holdindg the current [old_current , then the weight]
        public Dictionary<string, string> wall_paths;
        public List<string> path;

        public List<string> nodes = new List<string>();
        public List<string> visitied_nodes = new List<string>();
        public List<string> side_nodes = new List<string>();
        Dictionary<string, int> node_distance = new Dictionary<string, int>();
        Dictionary<string, string> node_predecesors = new Dictionary<string, string>();

        public void start_conditions()
        {

            maze_fill();

            start_end();
            type_gen();
            path_gen();

            end_checking();
            print_maze();

            connect_nodes();


            Console.CursorTop = 5;
            Console.CursorLeft = 0;
            print_maze();
            chosen_path();

        }

        public void maze_fill()
        {
            // populate the grid
            features = new int[height, width];
            Console.WriteLine("Maze filled");
        }
        public void start_end()
        {
            Random tile = new Random();

            // form (height,width)

            // start = edge tile randomly 
            // must be 0 on x and random on y

            start = tile.Next(1, (int)(height - 1) / 4).ToString() + ",0";

            // end = edge tile randomly 
            // must be width-1 on x and random on y

            end = tile.Next((int)height / 2, height - 1).ToString() + "," + (width - 1).ToString();
        }
        public void end_checking()
        {
            Random height_change = new Random();

            int end_x = int.Parse(end.Split(',')[1]);
            int end_y = int.Parse(end.Split(',')[0]);

            if (wall_paths[end_y + "," + (end_x - 1)] != "path")
            {
                wall_paths[end] = "wall";
                while (true)
                {
                    end_y = height_change.Next(1, height - 1);
                    if (end_y % 2 != 0) { continue; }
                    if (path.Contains(end_y + "," + (end_x - 1)))
                    {
                        wall_paths[end_y + "," + (end_x)] = "end";
                        end = (end_y) + "," + (end_x);
                        break;
                    };
                }

            }
        }
        public void type_gen()
        {
            wall_paths = new Dictionary<string, string>();
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    string i_index = i.ToString(); string j_index = j.ToString();
                    if (i_index + "," + j_index == start) { wall_paths.Add(start, "start"); }

                    else if (i_index + "," + j_index == end) { wall_paths.Add(end, "end"); }

                    else if (j == 0 || j == width - 1 || i == 0 || i == height - 1)
                    {
                        wall_paths.Add(i_index + "," + j_index, "wall");
                    }
                    else { wall_paths.Add(i_index + "," + j_index, "available"); }
                }
            }
        }
        public void check_for_frontiers(List<string> path, List<int> add, string current_path, string frontier_cell)
        {
            // determines what axis we are findind new values on x axis = 0 , y axis = 1
            int axis = 0;
            int index = -1;

            for (int j = 0; j < 4; j++)
            {
                index++;
                if (index == 2) { index = 0; axis++; }
                int new_pos = int.Parse(current_path.Split(',')[(axis + 1) % 2]) + add[index];

                if (new_pos < 0) { continue; }

                // validating the terms of new_pos
                if (axis == 0)
                {
                    if (new_pos > (width - 1)) { continue; }
                    frontier_cell = current_path.Split(',')[0] + "," + new_pos;
                }
                else if (axis == 1)
                {
                    if (new_pos > (height - 1)) { continue; }
                    frontier_cell = new_pos + "," + current_path.Split(',')[1];
                }

                int x = (int.Parse(frontier_cell.Split(',')[1]) + int.Parse(current_path.Split(',')[1])) / 2;
                int y = (int.Parse(frontier_cell.Split(',')[0]) + int.Parse(current_path.Split(',')[0])) / 2;
                string path_join = y.ToString() + "," + x.ToString();

                // validating if frontier_cell is already in the list
                if (wall_paths[frontier_cell] == "frontier" || wall_paths[frontier_cell] == "wall" || wall_paths[frontier_cell] == "path" || wall_paths[frontier_cell] == "start" || wall_paths[path_join] == "frontier" || wall_paths[path_join] == "wall" || wall_paths[path_join] == "path" || wall_paths[path_join] == "start") { continue; }
                if (frontier.ContainsKey(frontier_cell)) { continue; }

                // past here the frontier cell is valid
                frontier.Add(frontier_cell, current_path);
            }
        }


        public void connect_nodes()
        {
            bool found = false; int x_item = -1; int y_item = -1;

            nodes.Add(start);
            node_distance.Add(nodes[0], 0);

            while (!found)
            {
                if (found || nodes.Count == 0) { Console.WriteLine("no paths left"); Console.WriteLine(node_predecesors.Last()); break; }
                string current = nodes[0];
                int x = int.Parse(current.Split(',')[1]);
                int y = int.Parse(current.Split(',')[0]);

                List<string> possible_moves = new List<string>
                {
                    (y + 2) + "," + x,
                    (y - 2) + "," + x,
                    y + "," + (x + 2),
                    y + "," + (x - 2)
                };

                List<string> path_joins = new List<string>
                {
                    (y + 1) + "," + x,
                    (y - 1) + "," + x,
                    y + "," + (x + 1),
                    y + "," + (x - 1)
                };

                for (int index = 0; index < possible_moves.Count; index++)
                {
                    string item = possible_moves[index];
                    int previousx = x_item;
                    int previousy = y_item;
                    string previous = y_item + "," + x_item;

                    x_item = int.Parse(item.Split(',')[1]);
                    y_item = int.Parse(item.Split(',')[0]);


                    if (y_item < 0 || x_item < 0) { continue; }

                    if (item == end)
                    {
                        // found end
                        found = true;
                    }

                    if (!wall_paths.ContainsKey(item)) { continue; }
                    if (wall_paths[path_joins[index]] != "path") { continue; }


                    if (wall_paths[item] == "wall" || wall_paths[item] == "available")
                    {
                        possible_moves.Remove(item);
                        path_joins.RemoveAt(index);
                    }
                    else
                    {
                        if (!visitied_nodes.Contains(item) && !nodes.Contains(item))
                        {

                            // adding the next move to the checking queue
                            // adding the distance to the node_distance dictionary
                            if (item == start) { node_distance.Add(item, 0); }
                            else { node_distance.Add(item, node_distance[current] + 1); }

                            node_predecesors.Add(item, current);
                            side_nodes.Add(path_joins[index]);

                            if (end == (y_item + 1) + "," + (x_item + 1))
                            {

                                // adds the custom next cell as the key and then the item as the value (the predecessor)

                                node_predecesors.Add((y_item + 1) + "," + (x_item + 1), item);
                                side_nodes.Add((y_item + 1) + "," + (x_item));
                                found = true;
                            }
                            else if (end == (y_item - 1) + "," + (x_item + 1))
                            {
                                node_predecesors.Add((y_item - 1) + "," + (x_item + 1), item);

                                side_nodes.Add((y_item - 1) + "," + (x_item));
                                found = true;
                            }
                            else if (end == (y_item) + "," + (x_item + 1))
                            {
                                node_predecesors.Add(((y_item) + "," + (x_item + 1)), item);

                                side_nodes.Add(path_joins[index]);
                                found = true;
                            }

                            nodes.Add(item);

                        }
                    }
                }
                // adding the current node to the visited list
                // removing the current from the checking queue (nodes)
                visitied_nodes.Add(current);

                Console.CursorTop = int.Parse(current.Split(',')[0]) + 5;
                Console.CursorLeft = int.Parse(current.Split(',')[1]) * 2;
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.Write("  ");

                nodes.Remove(current);
               
            }

        }
        public void chosen_path()
        {
            string current = end;
            if (!wall_paths.ContainsKey(current)) { current = (int.Parse(end.Split(',')[0]) - 1) + "," + (int.Parse(end.Split(',')[1]) - 1); }
            if (!wall_paths.ContainsKey(current)) { current = (int.Parse(end.Split(',')[0]) + 1) + "," + (int.Parse(end.Split(',')[1]) - 1); }
            List<string> path_joining_adding = node_predecesors.Keys.ToList();

            while (current != start)
            {
                // 6 lines down is line 0 on the maze

                wall_paths[current] = "chosen";
                int x_c = int.Parse(current.Split(',')[1]) * 2;
                int y_c = int.Parse(current.Split(',')[0]);
                Console.CursorTop = y_c + 5;
                Console.CursorLeft = x_c;
                Console.BackgroundColor = ConsoleColor.Green;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("  ");
                Thread.Sleep(10);


                wall_paths[side_nodes[path_joining_adding.IndexOf(current)]] = "chosen";
                string current_join = side_nodes[path_joining_adding.IndexOf(current)];
                int x = int.Parse(current_join.Split(',')[1]) * 2;
                int y = int.Parse(current_join.Split(',')[0]);
                Console.CursorTop = y + 5;
                Console.CursorLeft = x;
                Console.BackgroundColor = ConsoleColor.Green;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("  ");
                Thread.Sleep(10);

                current = node_predecesors[current];
            }
        }

        public void path_gen()
        {
            Random path_gen = new Random();

            path = new List<string> { start };
            List<int> add = new List<int> { -2, 2 };
            int frontier_counter = 0;


            while (frontier_counter < path.Count)
            {
                string current_path = path[frontier_counter]; string frontier_cell = "";
                check_for_frontiers(path, add, current_path, frontier_cell);


                // calculate the mid point of current and chosen to find the path that joins them together
                int choice = path_gen.Next(0, frontier.Count);
                string chosen = frontier.ElementAt(choice).Key;
                string old_current = frontier.ElementAt(choice).Value;
                int x = (int.Parse(chosen.Split(',')[1]) + int.Parse(old_current.Split(',')[1])) / 2;
                int y = (int.Parse(chosen.Split(',')[0]) + int.Parse(old_current.Split(',')[0])) / 2;
                string path_join = y.ToString() + "," + x.ToString();

                path.Add(chosen);

                wall_paths[path_join] = "path";

                if (wall_paths[chosen] != "start" || wall_paths[chosen] != "end")
                {
                    wall_paths[chosen] = "path";
                }
                else
                {
                    wall_paths[chosen] = "end";
                }

                // adding the values of the path join to the key of the old current

                frontier.Remove(chosen);
                frontier_counter++;

                if (frontier_counter >= 2)
                {
                    if (frontier.Count == 0) { break; }
                }
            }
        } // works and generates paths although they may not be large scale random but rather local randomness
        public void print_maze()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    switch (wall_paths[(i.ToString() + "," + j.ToString())])
                    {
                        case "end":
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.BackgroundColor = ConsoleColor.DarkRed;
                            break;
                        case "start":
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.BackgroundColor = ConsoleColor.DarkRed;
                            break;
                        case "available":
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.BackgroundColor = ConsoleColor.DarkGray;
                            break;
                        case "wall":
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.BackgroundColor = ConsoleColor.Gray;
                            break;
                        case "path":
                            Console.ForegroundColor = ConsoleColor.Black;
                            break;
                        case "chosen":
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.BackgroundColor = ConsoleColor.Green;
                            break;
                    }

                    Console.Write(features[i, j] + " ");

                    Console.ResetColor();
                }
                Console.WriteLine();
            }

        }
        internal class Program
        {
            static void Main(string[] args)
            {
                maze_gen Maze = new maze_gen();

                Console.WriteLine("Enter width");
                Maze.width = int.Parse(Console.ReadLine());

                Console.WriteLine("Enter height");
                Maze.height = int.Parse(Console.ReadLine());

                Maze.start_conditions();

                Console.ReadKey();
            }
        }
    }
}