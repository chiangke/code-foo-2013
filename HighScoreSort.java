import java.io.File;
import java.io.FileNotFoundException;
import java.util.ArrayList;
import java.util.Scanner;
/*I used a mergesort algorithm to sort the high scores 
 *Mergesort has runtimes big-theta of n*logn, making it better than the worst case
 *for a quicksort (which has O(n^2) when all scores are already sorted)
 *Space complexity for mergesort is 2n
 *Time complexity can't be further reduced from O(nlogn), but space complexity
 *can be reduced by using an in-place mergesort or a heap sort
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
			tempString = scanner.nextLine();
			splitString = tempString.split(",");
			tempScore = new Score(Float.parseFloat(splitString[0]), splitString[1]);
			scoreList.add(tempScore);
		}
		for (Score s : scoreList) {
			System.out.println((s.getScore()) +"," + s.getName());
		}
		System.out.println("..");
		sortedList = mergesort(scoreList);
		for(Score s : sortedList) {
			System.out.println((s.getScore()) +"," + s.getName());
		}
		System.out.println("...");
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
			unsortedList.add(tempScore);
		}
		for(Score s : unsortedList) {
			System.out.println((s.getScore()) +"," + s.getName());
		}
		System.out.println("....");
		for(Score s : unsortedList) {
			insertScore(sortedList, s);
		}
		for(Score s : sortedList) {
			System.out.println((s.getScore()) +"," + s.getName());
		}
	}
	public void swap(ArrayList<Score> list, int swap1, int swap2) {
		Score temp = list.get(swap1);
		list.set(swap1, list.get(swap2));
		list.set(swap2, temp);
	}
	public int partition(ArrayList<Score> list, int left, int right, int pivotIndex) {
		Score pivotValue = list.get(pivotIndex);
		swap(list, pivotIndex, right);
		int storeIndex = left;
		for (int i=left; i<right; i++) {
			if (list.get(i).getScore() <= pivotValue.getScore()) {
				swap(list, i, storeIndex);
				storeIndex++;
			}
		}
		swap(list, storeIndex, right);
		return storeIndex;
	}
	public ArrayList<Score> inplacequicksort(ArrayList<Score> list, int left, int right) {
		if (left<right) {
			int pivotIndex = list.size()/2;
			int newPivotIndex = partition(list, left, right, pivotIndex);
			inplacequicksort(list, left, newPivotIndex-1);
			inplacequicksort(list, newPivotIndex+1, right);
		}
		return list;
	}
	public ArrayList<Score> mergesort(ArrayList<Score> list) {
		if (list.size() <=1) 
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
		left = mergesort(left);
		right = mergesort(right);
		return merge(left, right);
	}
	public ArrayList<Score> merge(ArrayList<Score> left, ArrayList<Score> right) {
		ArrayList<Score> scoreList = new ArrayList<Score>();
		while (left.size() > 0 || right.size() > 0) {
			if (left.size() > 0 && right.size() > 0) {
				if (left.get(0).getScore() >= right.get(0).getScore()) {
					scoreList.add(left.get(0));
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
	public ArrayList<Score> quicksort(ArrayList<Score> list) {
		if (list.size() <= 1)
			return list;
		int pivotIndex = list.size()/2;
	    ArrayList<Score> greater = new ArrayList<Score>();
	    ArrayList<Score> lesser = new ArrayList<Score>();
	    Score pivotScore = list.remove(pivotIndex);
	    float pivotValue = pivotScore.getScore();
	    float currentValue;
	    for(Score s : list)  {
	        currentValue = s.getScore();
	        if(currentValue <= pivotValue)
	            lesser.add(s);
	        else
	            greater.add(s);
	    }
	    ArrayList<Score> sorted = new ArrayList<Score>();
	    sorted.addAll(quicksort(greater));
	    sorted.add(pivotScore);
	    sorted.addAll(quicksort(lesser));
	    return sorted;
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
	public ArrayList<Score> insertionsort(ArrayList<Score> sortedlist, ArrayList<Score> unsortedlist) {
		ArrayList<Score> mergedList = new ArrayList<Score>();
		int i=0;
		return mergedList;
	}
	public void insert(int compareIndex, ArrayList<Score> a, int i) {
		while(i<compareIndex && a.get(i+1).getScore() < a.get(compareIndex).getScore()) {
			a.set(i+1,a.get(i));
			i++;
		}
		a.set(i,a.get(compareIndex));
	}
public static void main(String[] args) {
	HighScoreSort hss = new HighScoreSort();

}
}
