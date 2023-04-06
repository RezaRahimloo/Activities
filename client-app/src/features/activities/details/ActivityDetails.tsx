import { Button, Card } from "semantic-ui-react";
import { useStore } from "../../../app/strores/store";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import { Link, useParams } from "react-router-dom";
import { useEffect } from "react";
import { observer } from "mobx-react-lite";


export default observer(function ActivityDetails() {
    const { activityStore } = useStore();
    const { selectedActivity: activity, loadActivity, loadingInitial } = activityStore;
    const { id } = useParams<{ id: string }>();
    
    useEffect(() => {
        if (id) loadActivity(id);
    }, [id, loadActivity])

    if (loadingInitial || !activity) {// for not getting error 
        return <LoadingComponent />;
    }

    return (
        <Card fluid>
            <img src={`/assets/categoryImages/${activity.category}.jpg`} alt={`${activity.category}`} />
            <Card.Content>
                <Card.Header>{activity.title}</Card.Header>
                <Card.Meta>
                    <span>{activity.date}</span>
                </Card.Meta>
                <Card.Description>
                    {activity.description}
                </Card.Description>
            </Card.Content>
            <Card.Content extra>
                <Button.Group>
                    <Button as={Link} to={`/manage/${activity.id}`} basic color="blue" content='Edit' />
                    <Button as={Link} to={`/activities`} basic color="grey" content='Cancel' />
                </Button.Group>
            </Card.Content>
        </Card>
    )
});