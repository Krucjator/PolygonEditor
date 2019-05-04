# PolygonEditor

Application for displaying and editing polygons. Edition includes:
 - moving vertices 
 - adding and deleting vertices
 - restricting edges behaviour (e.g. making edge always horizontal)
 - locking and adjusting angles between edges

![alt text](https://raw.githubusercontent.com/Krucjator/PolygonEditor/master/Polygon1.png)

Additionally convex hull button trims the polygon, leaving only the vertices and edges which enclose the initial polygon.

![alt text](https://raw.githubusercontent.com/Krucjator/PolygonEditor/master/Polygon2.png)

# How to use

Keys:
	Left Mouse Click:
		On edge: create new vertex and drag to move it
		On vertex: drag to move vertex
	Right Mouse Click:
		On edge: show menu strip from whitch you can choose restriction to apply to that edge
		On vertex: delete vertex
	Middle Mouse Click:
		On vertex: lock angle between vertex's edges, also displays an editable textbox to set the angle (angle's change applies after moving any vertex)
		On edge: remove edge restriction

	Comments:
		to delete vertex's angle lock, you have to delete that vertex
