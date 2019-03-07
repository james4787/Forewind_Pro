using System.Collections.Generic;

public static class ListPool<T> {

	static Stack<List<T>> stack = new Stack<List<T>>();

    /// <summary>
    /// 从堆栈获取集合
    /// </summary>
    /// <returns>返回泛型列表</returns>
	public static List<T> Get () {
		if (stack.Count > 0) {
			return stack.Pop();
		}
		return new List<T>();
	}

    /// <summary>
    /// 清除列表并压栈
    /// </summary>
    /// <param name="list">入栈的列表对象</param>
	public static void Add (List<T> list) {
		list.Clear();
		stack.Push(list);
	}
}