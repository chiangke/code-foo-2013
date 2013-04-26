import java.io.File;
import java.io.FileNotFoundException;
import java.util.ArrayList;
import java.util.Scanner;
/*I used a mergesort algorithm to sort the high scores 
 *Mergesort has a runtime big-theta (n*logn), making it better than the worst case
 *for a quicksort (which has O(n^2) when all scores are already sorted)
 *Space complexity for mergesort is 2n due to the extra space needed in the arraylists
 *Time complexity can't be further reduced from O(nlogn), but space complexity
 *can be reduced by using an in-place mergesort or a heapsort
 *
 *For additional scores added after the list has been sorted, it is more efficient
 *to search through the sorted list and insert it in the proper location, for a runtime
 *of O(n), rather than running mergesort again with time O(n*logn)
 */

public class HighScoreSort {
	Scanner scanner;
	Score tempScore;
	String tempString;
	String splitString[];
	ArrayList<Score> scoreList;
	ArrayList<Score> sortedList;
	ArrayList<Score> unsortedList;
	public HighScoreSort() {
		try {
			scanner = new Scanner(new File("score-list.txt"));
		} catch(FileNotFoundException e) {
			System.out.println("Couldn't find file score-list.txt");
		}
		scoreList = new ArrayList<Score>();
		while(scanner.hasNextLine()) {
			tempString = scanner.nextLine(); //read in all lines (float,String)
			splitString = tempString.split(","); //split into 2 strings
			tempScore = new Score(Float.parseFloat(splitString[0]), splitString[1]); //create score object
			scoreList.add(tempScore); //add to list
		}
		System.out.println("List in original order: ");
		for (Score s : scoreList) { 
			System.out.println((s.getScore()) +"," + s.getName());
		}
		System.out.println("\nAfter mergesort:\n");
		sortedList = mergesort(scoreList);
		for(Score s : sortedList) {
			System.out.println((s.getScore()) +"," + s.getName());
		}
		try {
			scanner = new Scanner(new File("score-list2.txt"));
		} catch(FileNotFoundException e) {
			System.out.println("Couldn't find file score-list2.txt");
		}
		unsortedList = new ArrayList<Score>();
		while(scanner.hasNextLine()) {
			tempString = scanner.nextLine();
			splitString = tempString.split(",");
			tempScore = new Score(Float.parseFloat(splitString[0]), splitString[1]);
			unsortedList.add(tempScore); //adding additional scores to list
		}
		System.out.println("\nAdditional scores:");
		for(Score s : unsortedList) {
			System.out.println((s.getScore()) +"," + s.getName());
		}
		System.out.println("\nSimple insertion: ");
		for(Score s : unsortedList) {
			insertScore(sortedList, s);
		}
		for(Score s : sortedList) {
			System.out.println((s.getScore()) +"," + s.getName());
		}
	}
	public ArrayList<Score> mergesort(ArrayList<Score> list) {
		if (list.size() <=1) //base case 
			return list;
		ArrayList<Score> left, right;
		left = new ArrayList<Score>();
		right = new ArrayList<Score>();
		int indexMiddle = list.size()/2;
		for (int i=0; i<list.size(); i++) {
			if (i<indexMiddle)
				left.add(list.get(i));
			else
				right.add(list.get(i));
		}
		left = mergesort(left); //recursive calls
		right = mergesort(right); //for both halves
		return merge(left, right);
	}
	public ArrayList<Score> merge(ArrayList<Score> left, ArrayList<Score> right) {
		ArrayList<Score> scoreList = new ArrayList<Score>();
		while (left.size() > 0 || right.size() > 0) {
			if (left.size() > 0 && right.size() > 0) {
				if (left.get(0).getScore() >= right.get(0).getScore()) {
					scoreList.add(left.get(0));//add in decreasing order
					left.remove(0);
				}
				else {
					scoreList.add(right.get(0));
					right.remove(0);
				}
			}
			else if (left.size() > 0) {
				scoreList.add(left.remove(0));
			}
			else if (right.size() > 0) {
				scoreList.add(right.remove(0));
			}
		}
		return scoreList;
	}
	public ArrayList<Score> insertScore(ArrayList<Score> sortedList, Score s) {
		int i=0;
		while(sortedList.get(i).getScore() > s.getScore() && i<sortedList.size()-1) {
			i++;
		}
		if (i==sortedList.size()-1)
			sortedList.add(s);
		else
			sortedList.add(i,s);
		return sortedList;
	}
	public static void main(String[] args) {
		HighScoreSort hss = new HighScoreSort();
	}
}
