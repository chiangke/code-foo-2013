import java.util.*;

public class FamilyMember {
	String name;
	int generation;
	ArrayList<FamilyMember> children;
	public FamilyMember(String n , int g) {
		name = n;
		generation = g;
		children = new ArrayList<FamilyMember>();
	}
	public void addChild(FamilyMember c) {
		children.add(c);
	}
	public String getName() {
		return name;
	}
	public int getGeneration() {
		return generation;
	}
	public ArrayList<FamilyMember> getChildren() {
		return children;
	}
}
