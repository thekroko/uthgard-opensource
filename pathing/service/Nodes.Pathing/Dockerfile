FROM uthgard-nodes

VOLUME /pathing
EXPOSE 20023

WORKDIR ./Nodes.Pathing/bin/Release
RUN ln -s /pathing ./pathing
#ENTRYPOINT [ "mono",  "./Nodes.Pathing.exe" ]
ENTRYPOINT [ "bash", "-c", "while [ 1 ]; do mono ./Nodes.Pathing.exe; sleep 1s; done" ]
